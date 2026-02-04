using Microsoft.Extensions.AI;
using Dojo.Rag.Api.Models;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for generating embeddings from text using the configured AI provider.
/// </summary>
public interface IEmbeddingService
{
    Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default);
    Task<List<DocumentChunk>> EmbedChunksAsync(IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken = default);
}

public class EmbeddingService : IEmbeddingService
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(
        IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
        ILogger<EmbeddingService> logger)
    {
        _embeddingGenerator = embeddingGenerator;
        _logger = logger;
    }

    public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var result = await _embeddingGenerator.GenerateAsync(text, cancellationToken: cancellationToken);
        return result.Vector;
    }

    public async Task<IReadOnlyList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(
        IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        var textList = texts.ToList();
        _logger.LogInformation("Generating embeddings for {Count} texts", textList.Count);
        
        var results = await _embeddingGenerator.GenerateAsync(textList, cancellationToken: cancellationToken);
        return results.Select(r => r.Vector).ToList();
    }

    public async Task<List<DocumentChunk>> EmbedChunksAsync(
        IEnumerable<DocumentChunk> chunks, CancellationToken cancellationToken = default)
    {
        var chunkList = chunks.ToList();
        
        if (chunkList.Count == 0)
        {
            return chunkList;
        }
        
        _logger.LogInformation("Embedding {Count} document chunks", chunkList.Count);
        
        var texts = chunkList.Select(c => c.Content).ToList();
        var embeddings = await GenerateEmbeddingsAsync(texts, cancellationToken);
        
        for (int i = 0; i < chunkList.Count; i++)
        {
            chunkList[i].Embedding = embeddings[i];
        }
        
        return chunkList;
    }
}
