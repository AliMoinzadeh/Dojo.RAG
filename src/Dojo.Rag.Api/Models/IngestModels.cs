namespace Dojo.Rag.Api.Models;

public record IngestRequest(string FileName, string Content);

public record IngestResponse(
    string DocumentId,
    string FileName,
    int ChunksCreated,
    string CollectionName,
    long ProcessingTimeMs
);

public record CollectionInfo(
    string Name,
    string EmbeddingModel,
    int Dimensions,
    long DocumentCount,
    bool IsActive
);

public record EmbeddingVisualizationRequest(string? Query = null, int MaxPoints = 100);

public record EmbeddingPoint(
    string Id,
    string TextPreview,
    string SourceFile,
    float X,
    float Y,
    bool IsQuery = false,
    double? RelevanceScore = null
);

public record EmbeddingVisualizationResponse(
    List<EmbeddingPoint> Points,
    string CollectionName,
    string EmbeddingModel,
    int OriginalDimensions
);
