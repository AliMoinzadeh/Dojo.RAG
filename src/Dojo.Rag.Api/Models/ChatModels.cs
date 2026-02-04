namespace Dojo.Rag.Api.Models;

public record ChatRequest(string Message, bool IncludeDebugInfo = false);

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
