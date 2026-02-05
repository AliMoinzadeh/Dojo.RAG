using System.Text.RegularExpressions;
using Dojo.Rag.Api.Models;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for hybrid search combining vector similarity with keyword matching.
/// This helps find documents that match exact terms even when semantic similarity is low.
/// </summary>
public interface IHybridSearchService
{
    /// <summary>
    /// Performs hybrid search by combining vector scores with BM25-style keyword matching.
    /// </summary>
    Task<List<SearchResultItem>> SearchAsync(
        string query,
        List<DemoSentence> sentences,
        Dictionary<string, float[]> embeddings,
        ReadOnlyMemory<float> queryEmbedding,
        int topK = 5,
        float vectorWeight = 0.7f,
        float keywordWeight = 0.3f);
}

public class HybridSearchService : IHybridSearchService
{
    private readonly ILogger<HybridSearchService> _logger;

    public HybridSearchService(ILogger<HybridSearchService> logger)
    {
        _logger = logger;
    }

    public Task<List<SearchResultItem>> SearchAsync(
        string query,
        List<DemoSentence> sentences,
        Dictionary<string, float[]> embeddings,
        ReadOnlyMemory<float> queryEmbedding,
        int topK = 5,
        float vectorWeight = 0.7f,
        float keywordWeight = 0.3f)
    {
        _logger.LogInformation("Performing hybrid search for query: {Query}", query);

        var queryTokens = TokenizeText(query.ToLowerInvariant());
        var results = new List<(DemoSentence Sentence, double VectorScore, double KeywordScore, List<string> MatchedKeywords)>();

        foreach (var sentence in sentences)
        {
            if (!embeddings.TryGetValue(sentence.Id, out var embedding))
            {
                continue;
            }

            // Calculate vector similarity (cosine similarity)
            var vectorScore = CosineSimilarity(queryEmbedding.Span, embedding);

            // Calculate keyword score (BM25-inspired)
            var (keywordScore, matchedKeywords) = CalculateKeywordScore(queryTokens, sentence);

            results.Add((sentence, vectorScore, keywordScore, matchedKeywords));
        }

        // Combine scores with weights
        var rankedResults = results
            .Select(r => new
            {
                r.Sentence,
                CombinedScore = (r.VectorScore * vectorWeight) + (r.KeywordScore * keywordWeight),
                r.MatchedKeywords
            })
            .OrderByDescending(r => r.CombinedScore)
            .Take(topK)
            .Select(r => new SearchResultItem(
                r.Sentence.Id,
                r.Sentence.Text,
                r.Sentence.Category,
                Math.Round(r.CombinedScore, 4),
                r.MatchedKeywords
            ))
            .ToList();

        _logger.LogInformation("Hybrid search returned {Count} results", rankedResults.Count);

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

    private static (double Score, List<string> MatchedKeywords) CalculateKeywordScore(
        HashSet<string> queryTokens, 
        DemoSentence sentence)
    {
        var documentTokens = TokenizeText(sentence.Text.ToLowerInvariant());
        var tagTokens = sentence.Tags.Select(t => t.ToLowerInvariant()).ToHashSet();

        var matchedKeywords = new List<string>();
        int matches = 0;

        foreach (var queryToken in queryTokens)
        {
            // Check direct match in text
            if (documentTokens.Contains(queryToken))
            {
                matches++;
                matchedKeywords.Add(queryToken);
            }
            // Check match in tags (bonus for tag matches)
            else if (tagTokens.Contains(queryToken))
            {
                matches += 2; // Tags are more important
                matchedKeywords.Add($"[tag:{queryToken}]");
            }
            // Check partial matches (prefix matching)
            else
            {
                var partialMatch = documentTokens.FirstOrDefault(dt => 
                    dt.StartsWith(queryToken) || queryToken.StartsWith(dt));
                if (partialMatch != null)
                {
                    matches++;
                    matchedKeywords.Add($"~{partialMatch}");
                }
            }
        }

        // Normalize score: matches / query tokens count
        var score = queryTokens.Count > 0 ? (double)matches / queryTokens.Count : 0;
        
        // Apply diminishing returns for very long queries
        score = Math.Min(score, 1.0);

        return (score, matchedKeywords);
    }

    private static HashSet<string> TokenizeText(string text)
    {
        // Simple tokenization: split on non-word characters, filter short tokens
        var tokens = Regex.Split(text, @"\W+")
            .Where(t => t.Length >= 2)
            .Select(t => t.ToLowerInvariant())
            .ToHashSet();

        return tokens;
    }
}
