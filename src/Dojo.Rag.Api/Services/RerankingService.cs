using System.Globalization;
using Microsoft.Extensions.AI;
using Dojo.Rag.Api.Models;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for reranking search results using a cross-encoder style relevance pass.
/// Currently implemented via an LLM scoring step for demo purposes.
/// </summary>
public interface IRerankingService
{
    Task<List<SearchResultItem>> RerankAsync(
        string query,
        List<SearchResultItem> candidates,
        int topK = 5,
        CancellationToken cancellationToken = default);
}

public class RerankingService : IRerankingService
{
    private readonly IAIServiceFactory _aiServiceFactory;
    private readonly ILogger<RerankingService> _logger;

    public RerankingService(IAIServiceFactory aiServiceFactory, ILogger<RerankingService> logger)
    {
        _aiServiceFactory = aiServiceFactory;
        _logger = logger;
    }

    public async Task<List<SearchResultItem>> RerankAsync(
        string query,
        List<SearchResultItem> candidates,
        int topK = 5,
        CancellationToken cancellationToken = default)
    {
        if (candidates.Count == 0)
        {
            return candidates;
        }

        try
        {
            var chatClient = _aiServiceFactory.CreateChatClient();
            var prompt = BuildPrompt(query, candidates);

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, "You are a reranking model. You only output scores as instructed."),
                new(ChatRole.User, prompt)
            };

            var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
            var scoreMap = ParseScores(response.Text ?? string.Empty);

            if (scoreMap.Count == 0)
            {
                _logger.LogWarning("Reranking returned no valid scores. Falling back to original ordering.");
                return candidates.Take(topK).ToList();
            }

            var reranked = candidates
                .Select(candidate =>
                {
                    if (scoreMap.TryGetValue(candidate.Id, out var score))
                    {
                        return candidate with { Score = Math.Round(score, 4) };
                    }

                    return candidate;
                })
                .OrderByDescending(c => c.Score)
                .Take(topK)
                .ToList();

            return reranked;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Reranking failed. Falling back to original ordering.");
            return candidates.Take(topK).ToList();
        }
    }

    private static string BuildPrompt(string query, List<SearchResultItem> candidates)
    {
        var lines = new List<string>
        {
            "Score the relevance of each candidate to the query on a 0-1 scale.",
            "Return exactly one line per candidate in the format: <id>|<score>",
            "Only output these lines, no extra text.",
            $"Query: {query}",
            "Candidates:"
        };

        foreach (var candidate in candidates)
        {
            lines.Add($"- {candidate.Id}: {candidate.Text}");
        }

        return string.Join("\n", lines);
    }

    private static Dictionary<string, double> ParseScores(string responseText)
    {
        var scores = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var lines = responseText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            var parts = line.Split('|', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                continue;
            }

            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var score))
            {
                continue;
            }

            score = Math.Clamp(score, 0, 1);
            scores[parts[0]] = score;
        }

        return scores;
    }
}
