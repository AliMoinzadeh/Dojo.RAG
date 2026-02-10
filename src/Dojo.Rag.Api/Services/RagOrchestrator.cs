using System.Diagnostics;
using Microsoft.Extensions.AI;
using Dojo.Rag.Api.Models;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Main RAG orchestration service that combines search, context augmentation, and generation.
/// </summary>
public interface IRagOrchestrator
{
    Task<Models.ChatResponse> ChatAsync(Models.ChatRequest request, CancellationToken cancellationToken = default);
}

public class RagOrchestrator : IRagOrchestrator
{
    private readonly IVectorSearchService _searchService;
    private readonly IChatClient _chatClient;
    private readonly ITokenCounterService _tokenCounter;
    private readonly IAIServiceFactory _aiServiceFactory;
    private readonly IQueryExpansionService _queryExpansionService;
    private readonly IHyDEService _hydeService;
    private readonly IRerankingService _rerankingService;
    private readonly ILogger<RagOrchestrator> _logger;

    private const string SystemPromptTemplate = """
        You are a helpful assistant that answers questions based on the provided context.
        
        INSTRUCTIONS:
        - Answer the user's question using ONLY the information from the context below.
        - If the context doesn't contain enough information to answer the question, say "I don't have enough information to answer that question based on the available documents."
        - Be concise and accurate.
        - When relevant, mention which source document the information came from.
        
        CONTEXT:
        {0}
        """;

    public RagOrchestrator(
        IVectorSearchService searchService,
        IChatClient chatClient,
        ITokenCounterService tokenCounter,
        IAIServiceFactory aiServiceFactory,
        IQueryExpansionService queryExpansionService,
        IHyDEService hydeService,
        IRerankingService rerankingService,
        ILogger<RagOrchestrator> logger)
    {
        _searchService = searchService;
        _chatClient = chatClient;
        _tokenCounter = tokenCounter;
        _aiServiceFactory = aiServiceFactory;
        _queryExpansionService = queryExpansionService;
        _hydeService = hydeService;
        _rerankingService = rerankingService;
        _logger = logger;
    }

    public async Task<Models.ChatResponse> ChatAsync(Models.ChatRequest request, CancellationToken cancellationToken = default)
    {
        var totalSw = Stopwatch.StartNew();
        var metrics = new PipelineMetricsBuilder();
        
        _logger.LogInformation("Processing chat request: {Message}", request.Message);

        var enhancements = request.Enhancements ?? new RagSearchEnhancements();
        var searchQuery = request.Message;

        if (enhancements.UseQueryExpansion)
        {
            searchQuery = await _queryExpansionService.ExpandQueryAsync(request.Message, cancellationToken);
        }

        var embeddingInput = searchQuery;
        if (enhancements.UseHyDE)
        {
            embeddingInput = await _hydeService.GenerateHypotheticalDocumentAsync(searchQuery, cancellationToken);
        }
        
        // Step 1: Search for relevant chunks
        var searchSw = Stopwatch.StartNew();
        var retrievedChunks = await _searchService.SearchAsync(
            searchQuery,
            enhancements,
            embeddingInput,
            cancellationToken: cancellationToken);

        if (enhancements.UseReranking)
        {
            retrievedChunks = await RerankRetrievedChunksAsync(
                request.Message,
                retrievedChunks,
                cancellationToken);
        }
        searchSw.Stop();
        metrics.SearchTimeMs = searchSw.ElapsedMilliseconds;
        metrics.ChunksRetrieved = retrievedChunks.Count;
        
        _logger.LogInformation("Retrieved {Count} chunks in {ElapsedMs}ms",
            retrievedChunks.Count, searchSw.ElapsedMilliseconds);
        
        // Step 2: Build context from retrieved chunks
        var contextBuilder = new System.Text.StringBuilder();
        foreach (var chunk in retrievedChunks)
        {
            contextBuilder.AppendLine($"[Source: {chunk.SourceFileName}, Chunk {chunk.ChunkIndex}]");
            contextBuilder.AppendLine(chunk.Content);
            contextBuilder.AppendLine();
        }
        var context = contextBuilder.ToString();
        
        // Step 3: Build the augmented prompt
        var systemPrompt = string.Format(SystemPromptTemplate, context);
        
        // Step 4: Calculate token usage
        Models.TokenUsage? tokenUsage = null;
        if (request.IncludeDebugInfo)
        {
            tokenUsage = _tokenCounter.CalculateUsage(systemPrompt, context, request.Message);
        }
        
        // Step 5: Generate response
        var genSw = Stopwatch.StartNew();
        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, systemPrompt),
            new(ChatRole.User, request.Message)
        };
        
        var response = await _chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
        genSw.Stop();
        metrics.GenerationTimeMs = genSw.ElapsedMilliseconds;
        
        totalSw.Stop();
        metrics.TotalTimeMs = totalSw.ElapsedMilliseconds;
        metrics.EmbeddingModel = _aiServiceFactory.GetActiveEmbeddingModel();
        metrics.ChatModel = _aiServiceFactory.GetActiveChatModel();
        
        _logger.LogInformation("Generated response in {ElapsedMs}ms (total: {TotalMs}ms)",
            genSw.ElapsedMilliseconds, totalSw.ElapsedMilliseconds);
        
        return new Models.ChatResponse(
            Answer: response.Text ?? "No response generated",
            RetrievedChunks: retrievedChunks,
            TokenUsage: tokenUsage,
            Metrics: request.IncludeDebugInfo ? metrics.Build() : null
        );
    }

    private async Task<List<RetrievedChunk>> RerankRetrievedChunksAsync(
        string query,
        List<RetrievedChunk> chunks,
        CancellationToken cancellationToken)
    {
        if (chunks.Count == 0)
        {
            return chunks;
        }

        var candidates = chunks
            .Select(chunk => new SearchResultItem(
                chunk.Id,
                chunk.Content,
                chunk.SourceFileName,
                chunk.RelevanceScore,
                new List<string>()))
            .ToList();

        var reranked = await _rerankingService.RerankAsync(
            query,
            candidates,
            candidates.Count,
            cancellationToken);

        var lookup = chunks.ToDictionary(chunk => chunk.Id, StringComparer.Ordinal);
        var result = new List<RetrievedChunk>();

        foreach (var item in reranked)
        {
            if (lookup.TryGetValue(item.Id, out var chunk))
            {
                result.Add(chunk with { RelevanceScore = item.Score });
            }
        }

        return result;
    }

    private class PipelineMetricsBuilder
    {
        public long EmbeddingTimeMs { get; set; }
        public long SearchTimeMs { get; set; }
        public long GenerationTimeMs { get; set; }
        public long TotalTimeMs { get; set; }
        public int ChunksRetrieved { get; set; }
        public string EmbeddingModel { get; set; } = string.Empty;
        public string ChatModel { get; set; } = string.Empty;

        public PipelineMetrics Build() => new(
            EmbeddingTimeMs, SearchTimeMs, GenerationTimeMs, TotalTimeMs,
            ChunksRetrieved, EmbeddingModel, ChatModel
        );
    }
}
