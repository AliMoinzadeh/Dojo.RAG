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
    Task<List<RetrievedChunk>> SearchAsync(
        string query,
        RagSearchEnhancements? enhancements,
        string? embeddingInput = null,
        int? topK = null,
        CancellationToken cancellationToken = default);
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

    public Task<List<RetrievedChunk>> SearchAsync(string query, int? topK = null, CancellationToken cancellationToken = default)
    {
        return SearchAsync(query, enhancements: null, embeddingInput: null, topK: topK, cancellationToken: cancellationToken);
    }

    public async Task<List<RetrievedChunk>> SearchAsync(
        string query,
        RagSearchEnhancements? enhancements,
        string? embeddingInput = null,
        int? topK = null,
        CancellationToken cancellationToken = default)
    {
        var k = topK ?? _ragSettings.TopK;
        var useHybrid = enhancements?.UseHybridSearch == true;
        var useHnsw = enhancements?.UseHnswApproximate == true;
        var minScore = _ragSettings.MinRelevanceScore;
        var candidateK = k;

        if (useHnsw)
        {
            var efSearch = Math.Max(k, enhancements?.HnswEfSearch ?? k);
            candidateK = Math.Max(k, efSearch);
        }

        _logger.LogInformation(
            "Searching for query: {Query} with top-k: {TopK} (candidates: {Candidates})",
            query,
            k,
            candidateK);

        var embeddingText = string.IsNullOrWhiteSpace(embeddingInput) ? query : embeddingInput;
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(embeddingText, cancellationToken);

        var embeddingModel = _aiServiceFactory.GetActiveEmbeddingModel();
        var dimensions = _aiServiceFactory.GetActiveEmbeddingDimensions();
        var collection = await _vectorStoreManager.GetOrCreateCollectionAsync(embeddingModel, dimensions, cancellationToken);

        var searchOptions = new VectorSearchOptions<DocumentChunk>();
        var candidates = new List<(DocumentChunk Chunk, double VectorScore)>();

        await foreach (var result in collection.SearchAsync(queryEmbedding, candidateK, searchOptions, cancellationToken))
        {
            if (result.Score >= minScore)
            {
                candidates.Add((result.Record, result.Score ?? 0));
            }
        }

        List<RetrievedChunk> results;

        if (useHybrid)
        {
            var queryTokens = TokenizeText(query);
            results = candidates
                .Select(candidate => new
                {
                    candidate.Chunk,
                    CombinedScore = CombineScores(candidate.VectorScore, CalculateKeywordScore(queryTokens, candidate.Chunk.Content))
                })
                .Where(item => item.CombinedScore >= minScore)
                .OrderByDescending(item => item.CombinedScore)
                .Take(k)
                .Select(item => new RetrievedChunk(
                    Id: item.Chunk.Id,
                    Content: item.Chunk.Content,
                    SourceFileName: item.Chunk.SourceFileName,
                    ChunkIndex: item.Chunk.ChunkIndex,
                    RelevanceScore: item.CombinedScore
                ))
                .ToList();
        }
        else
        {
            results = candidates
                .OrderByDescending(candidate => candidate.VectorScore)
                .Take(k)
                .Select(candidate => new RetrievedChunk(
                    Id: candidate.Chunk.Id,
                    Content: candidate.Chunk.Content,
                    SourceFileName: candidate.Chunk.SourceFileName,
                    ChunkIndex: candidate.Chunk.ChunkIndex,
                    RelevanceScore: candidate.VectorScore
                ))
                .ToList();
        }

        _logger.LogInformation("Found {Count} relevant chunks (min score: {MinScore})",
            results.Count, minScore);

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

    private static double CombineScores(double vectorScore, double keywordScore)
    {
        const double vectorWeight = 0.7;
        const double keywordWeight = 0.3;
        return (vectorScore * vectorWeight) + (keywordScore * keywordWeight);
    }

    private static double CalculateKeywordScore(HashSet<string> queryTokens, string content)
    {
        if (queryTokens.Count == 0)
        {
            return 0;
        }

        var documentTokens = TokenizeText(content);
        var matches = 0;

        foreach (var queryToken in queryTokens)
        {
            if (documentTokens.Contains(queryToken))
            {
                matches++;
            }
            else
            {
                var partialMatch = documentTokens.FirstOrDefault(token =>
                    token.StartsWith(queryToken, StringComparison.OrdinalIgnoreCase)
                    || queryToken.StartsWith(token, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(partialMatch))
                {
                    matches++;
                }
            }
        }

        var score = (double)matches / queryTokens.Count;
        return Math.Min(score, 1.0);
    }

    private static HashSet<string> TokenizeText(string text)
    {
        return text
            .Split(new[] { ' ', '\n', '\r', '\t', '.', ',', ';', ':', '!', '?', '"', '\'', '(', ')', '[', ']', '{', '}', '/', '\\', '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(token => token.Length >= 2)
            .Select(token => token.ToLowerInvariant())
            .ToHashSet();
    }
}
