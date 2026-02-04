using Microsoft.Extensions.VectorData;

namespace Dojo.Rag.Api.Models;

/// <summary>
/// Represents a chunk of a document stored in the vector database.
/// Collection name format: documents_{embeddingModelName} (e.g., documents_text-embedding-3-small)
/// </summary>
public class DocumentChunk
{
    [VectorStoreKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [VectorStoreData(IsIndexed = true)]
    public string SourceDocument { get; set; } = string.Empty;
    
    [VectorStoreData(IsIndexed = true)]
    public string SourceFileName { get; set; } = string.Empty;
    
    [VectorStoreData(IsFullTextIndexed = true)]
    public string Content { get; set; } = string.Empty;
    
    [VectorStoreData]
    public int ChunkIndex { get; set; }
    
    [VectorStoreData]
    public int StartCharIndex { get; set; }
    
    [VectorStoreData]
    public int EndCharIndex { get; set; }
    
    [VectorStoreData]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [VectorStoreVector(Dimensions: 1536, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float>? Embedding { get; set; }
}

/// <summary>
/// Factory to create DocumentChunk instances with correct embedding dimensions
/// </summary>
public static class DocumentChunkFactory
{
    public static DocumentChunk Create(
        string content,
        string sourceDocument,
        string sourceFileName,
        int chunkIndex,
        int startCharIndex,
        int endCharIndex,
        ReadOnlyMemory<float>? embedding = null)
    {
        return new DocumentChunk
        {
            Id = Guid.NewGuid().ToString(),
            Content = content,
            SourceDocument = sourceDocument,
            SourceFileName = sourceFileName,
            ChunkIndex = chunkIndex,
            StartCharIndex = startCharIndex,
            EndCharIndex = endCharIndex,
            Embedding = embedding,
            CreatedAt = DateTime.UtcNow
        };
    }
}
