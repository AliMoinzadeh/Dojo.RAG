using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;
using Dojo.Rag.Api.Configuration;
using Dojo.Rag.Api.Models;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Manages vector store collections, supporting multiple embedding models with separate collections.
/// Collection naming convention: documents_{embeddingModelName}
/// </summary>
public interface IVectorStoreManager
{
    Task<VectorStoreCollection<string, DocumentChunk>> GetOrCreateCollectionAsync(
        string embeddingModel, int dimensions, CancellationToken cancellationToken = default);
    Task<IEnumerable<CollectionInfo>> ListCollectionsAsync(CancellationToken cancellationToken = default);
    Task<bool> CollectionExistsAsync(string embeddingModel, CancellationToken cancellationToken = default);
    Task<long> GetDocumentCountAsync(string embeddingModel, CancellationToken cancellationToken = default);
    Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default);
    Task DeleteAllCollectionsAsync(CancellationToken cancellationToken = default);
    string GetCollectionName(string embeddingModel);
}

public class VectorStoreManager : IVectorStoreManager
{
    private readonly VectorStoreSettings _settings;
    private readonly IAIServiceFactory _aiServiceFactory;
    private readonly ILogger<VectorStoreManager> _logger;
    private readonly Dictionary<string, VectorStoreCollection<string, DocumentChunk>> _collections = new();
    private readonly Dictionary<string, (string Model, int Dimensions)> _collectionMetadata = new();
    private readonly object _lock = new();
    
    // In-memory store for InMemory provider
    private InMemoryVectorStore? _inMemoryStore;
    
    // Qdrant client for Qdrant provider
    private QdrantClient? _qdrantClient;
    private QdrantVectorStore? _qdrantStore;

    public VectorStoreManager(
        IOptions<VectorStoreSettings> settings,
        IAIServiceFactory aiServiceFactory,
        ILogger<VectorStoreManager> logger)
    {
        _settings = settings.Value;
        _aiServiceFactory = aiServiceFactory;
        _logger = logger;
        
        InitializeVectorStore();
    }

    private void InitializeVectorStore()
    {
        switch (_settings.Provider)
        {
            case "InMemory":
                _inMemoryStore = new InMemoryVectorStore();
                _logger.LogInformation("Initialized InMemory vector store");
                break;
            case "Qdrant":
                _qdrantClient = new QdrantClient(_settings.Qdrant.Host, _settings.Qdrant.Port);
                _qdrantStore = new QdrantVectorStore(_qdrantClient, ownsClient: true);
                _logger.LogInformation("Initialized Qdrant vector store at {Host}:{Port}", 
                    _settings.Qdrant.Host, _settings.Qdrant.Port);
                break;
            default:
                throw new ArgumentException($"Unknown vector store provider: {_settings.Provider}");
        }
    }

    public string GetCollectionName(string embeddingModel)
    {
        // Sanitize model name for collection naming (replace special chars)
        var sanitized = embeddingModel
            .Replace("/", "-")
            .Replace(":", "-")
            .Replace(".", "-")
            .ToLowerInvariant();
        return $"documents_{sanitized}";
    }

    public async Task<VectorStoreCollection<string, DocumentChunk>> GetOrCreateCollectionAsync(
        string embeddingModel, int dimensions, CancellationToken cancellationToken = default)
    {
        var collectionName = GetCollectionName(embeddingModel);
        
        lock (_lock)
        {
            if (_collections.TryGetValue(collectionName, out var existingCollection))
            {
                return existingCollection;
            }
        }

        _logger.LogInformation("Creating collection {CollectionName} for model {Model} with {Dimensions} dimensions",
            collectionName, embeddingModel, dimensions);

        VectorStoreCollection<string, DocumentChunk> collection = _settings.Provider switch
        {
            "InMemory" => _inMemoryStore!.GetCollection<string, DocumentChunk>(collectionName),
            "Qdrant" => _qdrantStore!.GetCollection<string, DocumentChunk>(collectionName),
            _ => throw new ArgumentException($"Unknown vector store provider: {_settings.Provider}")
        };

        await collection.EnsureCollectionExistsAsync(cancellationToken);

        lock (_lock)
        {
            _collections[collectionName] = collection;
            _collectionMetadata[collectionName] = (embeddingModel, dimensions);
        }

        return collection;
    }

    public Task<IEnumerable<CollectionInfo>> ListCollectionsAsync(CancellationToken cancellationToken = default)
    {
        var activeModel = _aiServiceFactory.GetActiveEmbeddingModel();
        
        lock (_lock)
        {
            var collections = _collectionMetadata.Select(kvp => new CollectionInfo(
                Name: kvp.Key,
                EmbeddingModel: kvp.Value.Model,
                Dimensions: kvp.Value.Dimensions,
                DocumentCount: 0, // Would need async enumeration to count
                IsActive: kvp.Value.Model == activeModel
            ));
            
            return Task.FromResult(collections);
        }
    }

    public Task<bool> CollectionExistsAsync(string embeddingModel, CancellationToken cancellationToken = default)
    {
        var collectionName = GetCollectionName(embeddingModel);
        
        lock (_lock)
        {
            return Task.FromResult(_collections.ContainsKey(collectionName));
        }
    }

    public async Task<long> GetDocumentCountAsync(string embeddingModel, CancellationToken cancellationToken = default)
    {
        var collectionName = GetCollectionName(embeddingModel);
        
        lock (_lock)
        {
            if (!_collections.ContainsKey(collectionName))
            {
                return 0;
            }
        }

        // For InMemory, we need to enumerate - this is simplified
        // In production, you'd use proper count APIs
        return 0;
    }

    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting collection {CollectionName}", collectionName);
        
        VectorStoreCollection<string, DocumentChunk>? collection = null;
        
        lock (_lock)
        {
            if (_collections.TryGetValue(collectionName, out collection))
            {
                _collections.Remove(collectionName);
                _collectionMetadata.Remove(collectionName);
            }
        }

        if (collection != null)
        {
            try
            {
                // The collection interface uses EnsureCollectionDeletedAsync or similar
                // We need to recreate and delete via the store directly
                switch (_settings.Provider)
                {
                    case "InMemory":
                        // InMemory collections are already removed from our dictionary
                        // Re-getting and deleting ensures the underlying store is cleared
                        var inMemoryCol = _inMemoryStore!.GetCollection<string, DocumentChunk>(collectionName);
                        await inMemoryCol.EnsureCollectionDeletedAsync(cancellationToken);
                        break;
                    case "Qdrant":
                        await _qdrantClient!.DeleteCollectionAsync(collectionName);
                        break;
                }
                _logger.LogInformation("Deleted collection {CollectionName}", collectionName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete collection {CollectionName} from vector store", collectionName);
            }
        }
    }

    public async Task DeleteAllCollectionsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting all collections");
        
        List<string> collectionNames;
        lock (_lock)
        {
            collectionNames = _collections.Keys.ToList();
        }

        foreach (var name in collectionNames)
        {
            await DeleteCollectionAsync(name, cancellationToken);
        }
    }
}
