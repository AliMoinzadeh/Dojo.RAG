using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Dojo.Rag.Api.Configuration;
using Dojo.Rag.Api.Models;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for searching the vector store for relevant document chunks.
/// </summary>
public interface IVectorSearchService
{
    Task<List<RetrievedChunk>> SearchAsync(string query, int? topK = null, CancellationToken cancellationToken = default);
    Task<List<(DocumentChunk Chunk, float[] Embedding)>> GetAllChunksWithEmbeddingsAsync(int maxCount = 100, CancellationToken cancellationToken = default);
}

public class VectorSearchService : IVectorSearchService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorStoreManager _vectorStoreManager;
    private readonly IAIServiceFactory _aiServiceFactory;
    private readonly RagSettings _ragSettings;
    private readonly ILogger<VectorSearchService> _logger;

    public VectorSearchService(
        IEmbeddingService embeddingService,
        IVectorStoreManager vectorStoreManager,
        IAIServiceFactory aiServiceFactory,
        IOptions<RagSettings> ragSettings,
        ILogger<VectorSearchService> logger)
    {
        _embeddingService = embeddingService;
        _vectorStoreManager = vectorStoreManager;
        _aiServiceFactory = aiServiceFactory;
        _ragSettings = ragSettings.Value;
        _logger = logger;
    }

    public async Task<List<RetrievedChunk>> SearchAsync(string query, int? topK = null, CancellationToken cancellationToken = default)
    {
        var k = topK ?? _ragSettings.TopK;
        
        _logger.LogInformation("Searching for query: {Query} with top-k: {TopK}", query, k);
        
        // Generate query embedding
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query, cancellationToken);
        
        // Get the collection for the current embedding model
        var embeddingModel = _aiServiceFactory.GetActiveEmbeddingModel();
        var dimensions = _aiServiceFactory.GetActiveEmbeddingDimensions();
        var collection = await _vectorStoreManager.GetOrCreateCollectionAsync(embeddingModel, dimensions, cancellationToken);
        
        // Search for similar vectors
        var searchOptions = new VectorSearchOptions<DocumentChunk>();
        
        var results = new List<RetrievedChunk>();
        
        await foreach (var result in collection.SearchAsync(queryEmbedding, k, searchOptions, cancellationToken))
        {
            if (result.Score >= _ragSettings.MinRelevanceScore)
            {
                results.Add(new RetrievedChunk(
                    Id: result.Record.Id,
                    Content: result.Record.Content,
                    SourceFileName: result.Record.SourceFileName,
                    ChunkIndex: result.Record.ChunkIndex,
                    RelevanceScore: result.Score ?? 0
                ));
            }
        }
        
        _logger.LogInformation("Found {Count} relevant chunks (min score: {MinScore})",
            results.Count, _ragSettings.MinRelevanceScore);
        
        return results;
    }

    public async Task<List<(DocumentChunk Chunk, float[] Embedding)>> GetAllChunksWithEmbeddingsAsync(
        int maxCount = 100, CancellationToken cancellationToken = default)
    {
        var embeddingModel = _aiServiceFactory.GetActiveEmbeddingModel();
        var dimensions = _aiServiceFactory.GetActiveEmbeddingDimensions();
        var collection = await _vectorStoreManager.GetOrCreateCollectionAsync(embeddingModel, dimensions, cancellationToken);
        
        var results = new List<(DocumentChunk, float[])>();
        
        // Note: This is a simplified implementation
        // In production, you'd use proper pagination/streaming
        // For now, we rely on the chunks being stored and retrievable
        
        return results;
    }
}
