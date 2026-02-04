using System.Diagnostics;
using Dojo.Rag.Api.Models;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for ingesting documents: chunking, embedding, and storing in vector database.
/// Handles auto-ingestion when switching embedding models with empty collections.
/// </summary>
public interface IDocumentIngestionService
{
    Task<IngestResponse> IngestDocumentAsync(IngestRequest request, CancellationToken cancellationToken = default);
    Task<IngestResponse> ReIngestDocumentAsync(string documentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SourceDocument>> GetSourceDocumentsAsync(CancellationToken cancellationToken = default);
    Task AutoIngestIfNeededAsync(CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync(string documentId, CancellationToken cancellationToken = default);
    Task DeleteAllDocumentsAsync(CancellationToken cancellationToken = default);
}

public class DocumentIngestionService : IDocumentIngestionService
{
    private readonly IDocumentChunker _chunker;
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStoreManager _vectorStoreManager;
    private readonly IAIServiceFactory _aiServiceFactory;
    private readonly ILogger<DocumentIngestionService> _logger;
    
    // In-memory storage for source documents (for demo purposes)
    // In production, use a proper database
    private static readonly Dictionary<string, SourceDocument> _sourceDocuments = new();
    private static readonly object _lock = new();

    public DocumentIngestionService(
        IDocumentChunker chunker,
        IEmbeddingService embeddingService,
        IVectorStoreManager vectorStoreManager,
        IAIServiceFactory aiServiceFactory,
        ILogger<DocumentIngestionService> logger)
    {
        _chunker = chunker;
        _embeddingService = embeddingService;
        _vectorStoreManager = vectorStoreManager;
        _aiServiceFactory = aiServiceFactory;
        _logger = logger;
    }

    public async Task<IngestResponse> IngestDocumentAsync(IngestRequest request, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        
        _logger.LogInformation("Ingesting document: {FileName}", request.FileName);
        
        // Create source document
        var sourceDoc = new SourceDocument
        {
            FileName = request.FileName,
            Content = request.Content
        };
        
        // Store source document for potential re-ingestion
        lock (_lock)
        {
            _sourceDocuments[sourceDoc.Id] = sourceDoc;
        }
        
        // Chunk the document
        var chunks = _chunker.ChunkDocument(sourceDoc).ToList();
        
        // Generate embeddings
        var embeddedChunks = await _embeddingService.EmbedChunksAsync(chunks, cancellationToken);
        
        // Get the collection for the current embedding model
        var embeddingModel = _aiServiceFactory.GetActiveEmbeddingModel();
        var dimensions = _aiServiceFactory.GetActiveEmbeddingDimensions();
        var collection = await _vectorStoreManager.GetOrCreateCollectionAsync(embeddingModel, dimensions, cancellationToken);
        var collectionName = _vectorStoreManager.GetCollectionName(embeddingModel);
        
        // Store chunks in vector store
        foreach (var chunk in embeddedChunks)
        {
            await collection.UpsertAsync(chunk, cancellationToken: cancellationToken);
        }
        
        sw.Stop();
        
        _logger.LogInformation("Ingested {ChunkCount} chunks from {FileName} into collection {Collection} in {ElapsedMs}ms",
            embeddedChunks.Count, request.FileName, collectionName, sw.ElapsedMilliseconds);
        
        return new IngestResponse(
            DocumentId: sourceDoc.Id,
            FileName: sourceDoc.FileName,
            ChunksCreated: embeddedChunks.Count,
            CollectionName: collectionName,
            ProcessingTimeMs: sw.ElapsedMilliseconds
        );
    }

    public async Task<IngestResponse> ReIngestDocumentAsync(string documentId, CancellationToken cancellationToken = default)
    {
        SourceDocument? sourceDoc;
        lock (_lock)
        {
            _sourceDocuments.TryGetValue(documentId, out sourceDoc);
        }
        
        if (sourceDoc == null)
        {
            throw new ArgumentException($"Document with ID {documentId} not found");
        }
        
        return await IngestDocumentAsync(
            new IngestRequest(sourceDoc.FileName, sourceDoc.Content),
            cancellationToken);
    }

    public Task<IEnumerable<SourceDocument>> GetSourceDocumentsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_sourceDocuments.Values.AsEnumerable());
        }
    }

    public async Task AutoIngestIfNeededAsync(CancellationToken cancellationToken = default)
    {
        var embeddingModel = _aiServiceFactory.GetActiveEmbeddingModel();
        var collectionExists = await _vectorStoreManager.CollectionExistsAsync(embeddingModel, cancellationToken);
        
        if (!collectionExists)
        {
            _logger.LogInformation("Collection for model {Model} does not exist, checking for documents to auto-ingest",
                embeddingModel);
            
            IEnumerable<SourceDocument> sourceDocs;
            lock (_lock)
            {
                sourceDocs = _sourceDocuments.Values.ToList();
            }
            
            if (sourceDocs.Any())
            {
                _logger.LogInformation("Auto-ingesting {Count} documents for model {Model}",
                    sourceDocs.Count(), embeddingModel);
                
                foreach (var doc in sourceDocs)
                {
                    await IngestDocumentAsync(
                        new IngestRequest(doc.FileName, doc.Content),
                        cancellationToken);
                }
            }
        }
    }

    public Task DeleteDocumentAsync(string documentId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_sourceDocuments.ContainsKey(documentId))
            {
                _sourceDocuments.Remove(documentId);
                _logger.LogInformation("Deleted source document {DocumentId}", documentId);
            }
            else
            {
                throw new ArgumentException($"Document with ID {documentId} not found");
            }
        }
        return Task.CompletedTask;
    }

    public async Task DeleteAllDocumentsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting all documents and collections");
        
        lock (_lock)
        {
            _sourceDocuments.Clear();
        }

        // Also delete all vector collections
        await _vectorStoreManager.DeleteAllCollectionsAsync(cancellationToken);
        
        _logger.LogInformation("All documents and collections deleted");
    }
}
