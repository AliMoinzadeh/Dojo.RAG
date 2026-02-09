using Dojo.Rag.Api.Models;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for multi-vector search using separate embeddings per sentence.
/// </summary>
public interface IMultiVectorSearchService
{
    Task<List<SearchResultItem>> SearchAsync(
        List<DemoSentence> sentences,
        Dictionary<string, float[]> contentEmbeddings,
        Dictionary<string, float[]> tagEmbeddings,
        ReadOnlyMemory<float> queryEmbedding,
        int topK = 5,
        float contentWeight = 0.75f,
        float tagWeight = 0.25f);
}

public class MultiVectorSearchService : IMultiVectorSearchService
{
    private readonly ILogger<MultiVectorSearchService> _logger;

    public MultiVectorSearchService(ILogger<MultiVectorSearchService> logger)
    {
        _logger = logger;
    }

    public Task<List<SearchResultItem>> SearchAsync(
        List<DemoSentence> sentences,
        Dictionary<string, float[]> contentEmbeddings,
        Dictionary<string, float[]> tagEmbeddings,
        ReadOnlyMemory<float> queryEmbedding,
        int topK = 5,
        float contentWeight = 0.75f,
        float tagWeight = 0.25f)
    {
        _logger.LogInformation("Performing multi-vector search for {Count} sentences", sentences.Count);

        var results = new List<(DemoSentence Sentence, double CombinedScore)>();

        foreach (var sentence in sentences)
        {
            if (!contentEmbeddings.TryGetValue(sentence.Id, out var contentEmbedding))
            {
                continue;
            }

            tagEmbeddings.TryGetValue(sentence.Id, out var tagEmbedding);

            var contentScore = CosineSimilarity(queryEmbedding.Span, contentEmbedding);
            var tagScore = tagEmbedding == null ? 0 : CosineSimilarity(queryEmbedding.Span, tagEmbedding);
            var combined = (contentScore * contentWeight) + (tagScore * tagWeight);

            results.Add((sentence, combined));
        }

        var rankedResults = results
            .OrderByDescending(r => r.CombinedScore)
            .Take(topK)
            .Select(r => new SearchResultItem(
                r.Sentence.Id,
                r.Sentence.Text,
                r.Sentence.Category,
                Math.Round(r.CombinedScore, 4),
                new List<string>()
            ))
            .ToList();

        return Task.FromResult(rankedResults);
    }

    private static double CosineSimilarity(ReadOnlySpan<float> a, float[] b)
    {
        if (a.Length != b.Length)
        {
            return 0;
        }

        double dotProduct = 0;
        double normA = 0;
        double normB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        var denominator = Math.Sqrt(normA) * Math.Sqrt(normB);
        return denominator == 0 ? 0 : dotProduct / denominator;
    }
}
