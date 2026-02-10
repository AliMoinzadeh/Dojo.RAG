namespace Dojo.Rag.Api.Models;

public record ChatRequest(
    string Message,
    bool IncludeDebugInfo = false,
    RagSearchEnhancements? Enhancements = null);

public record RagSearchEnhancements(
    bool UseHybridSearch = false,
    bool UseQueryExpansion = false,
    bool UseReranking = false,
    bool UseHyDE = false,
    bool UseHnswApproximate = false,
    int HnswEfSearch = 32
);

public record ChatResponse(
    string Answer,
    List<RetrievedChunk> RetrievedChunks,
    TokenUsage? TokenUsage,
    PipelineMetrics? Metrics
);

public record RetrievedChunk(
    string Id,
    string Content,
    string SourceFileName,
    int ChunkIndex,
    double RelevanceScore
);

public record TokenUsage(
    int SystemPromptTokens,
    int ContextTokens,
    int QueryTokens,
    int TotalInputTokens,
    int MaxContextTokens,
    double UsagePercentage
);

public record PipelineMetrics(
    long EmbeddingTimeMs,
    long SearchTimeMs,
    long GenerationTimeMs,
    long TotalTimeMs,
    int ChunksRetrieved,
    string EmbeddingModel,
    string ChatModel
);
