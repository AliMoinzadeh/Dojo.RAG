using Tiktoken;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for counting tokens using Tiktoken for context window visualization.
/// </summary>
public interface ITokenCounterService
{
    int CountTokens(string text);
    Models.TokenUsage CalculateUsage(string systemPrompt, string context, string query, int maxContextTokens = 128000);
}

public class TokenCounterService : ITokenCounterService
{
    private readonly Encoder _encoder;
    private readonly ILogger<TokenCounterService> _logger;

    public TokenCounterService(ILogger<TokenCounterService> logger)
    {
        // Use cl100k_base encoding (used by GPT-4, GPT-3.5-turbo)
        _encoder = ModelToEncoder.For("gpt-4o");
        _logger = logger;
    }

    public int CountTokens(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }
        
        return _encoder.CountTokens(text);
    }

    public Models.TokenUsage CalculateUsage(string systemPrompt, string context, string query, int maxContextTokens = 128000)
    {
        var systemTokens = CountTokens(systemPrompt);
        var contextTokens = CountTokens(context);
        var queryTokens = CountTokens(query);
        var totalTokens = systemTokens + contextTokens + queryTokens;
        
        var usage = new Models.TokenUsage(
            SystemPromptTokens: systemTokens,
            ContextTokens: contextTokens,
            QueryTokens: queryTokens,
            TotalInputTokens: totalTokens,
            MaxContextTokens: maxContextTokens,
            UsagePercentage: Math.Round((double)totalTokens / maxContextTokens * 100, 2)
        );
        
        _logger.LogDebug("Token usage - System: {System}, Context: {Context}, Query: {Query}, Total: {Total}/{Max} ({Pct}%)",
            systemTokens, contextTokens, queryTokens, totalTokens, maxContextTokens, usage.UsagePercentage);
        
        return usage;
    }
}
