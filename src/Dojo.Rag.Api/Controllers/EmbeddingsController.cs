using Microsoft.AspNetCore.Mvc;
using Dojo.Rag.Api.Models;
using Dojo.Rag.Api.Services;

namespace Dojo.Rag.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmbeddingsController : ControllerBase
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorSearchService _searchService;
    private readonly IAIServiceFactory _aiServiceFactory;
    private readonly IVectorStoreManager _vectorStoreManager;
    private readonly ILogger<EmbeddingsController> _logger;

    public EmbeddingsController(
        IEmbeddingService embeddingService,
        IVectorSearchService searchService,
        IAIServiceFactory aiServiceFactory,
        IVectorStoreManager vectorStoreManager,
        ILogger<EmbeddingsController> logger)
    {
        _embeddingService = embeddingService;
        _searchService = searchService;
        _aiServiceFactory = aiServiceFactory;
        _vectorStoreManager = vectorStoreManager;
        _logger = logger;
    }

    /// <summary>
    /// Get embedding visualization data with optional query highlighting.
    /// Returns 2D projected points for visualization.
    /// </summary>
    [HttpPost("visualize")]
    public async Task<ActionResult<EmbeddingVisualizationResponse>> Visualize(
        [FromBody] EmbeddingVisualizationRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var embeddingModel = _aiServiceFactory.GetActiveEmbeddingModel();
            var dimensions = _aiServiceFactory.GetActiveEmbeddingDimensions();
            var collectionName = _vectorStoreManager.GetCollectionName(embeddingModel);

            // Get all chunks with embeddings
            var chunksWithEmbeddings = await _searchService.GetAllChunksWithEmbeddingsAsync(
                request.MaxPoints, cancellationToken);

            var points = new List<EmbeddingPoint>();

            // Project embeddings to 2D using simple PCA-like projection
            // (In production, you'd use UMAP or t-SNE on the frontend)
            foreach (var (chunk, embedding) in chunksWithEmbeddings)
            {
                var (x, y) = SimpleProject(embedding);
                points.Add(new EmbeddingPoint(
                    Id: chunk.Id,
                    TextPreview: chunk.Content.Length > 100 ? chunk.Content[..100] + "..." : chunk.Content,
                    SourceFile: chunk.SourceFileName,
                    X: x,
                    Y: y,
                    IsQuery: false,
                    RelevanceScore: null
                ));
            }

            // If query provided, add query point and calculate relevance
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(request.Query, cancellationToken);
                var queryEmbeddingArray = queryEmbedding.ToArray();
                var (qx, qy) = SimpleProject(queryEmbeddingArray);
                
                points.Add(new EmbeddingPoint(
                    Id: "query",
                    TextPreview: request.Query,
                    SourceFile: "Query",
                    X: qx,
                    Y: qy,
                    IsQuery: true,
                    RelevanceScore: null
                ));

                // Get search results to add relevance scores
                var searchResults = await _searchService.SearchAsync(request.Query, cancellationToken: cancellationToken);
                var relevanceMap = searchResults.ToDictionary(r => r.Id, r => r.RelevanceScore);
                
                // Update points with relevance scores
                points = points.Select(p => p.IsQuery ? p : p with 
                { 
                    RelevanceScore = relevanceMap.TryGetValue(p.Id, out var score) ? score : null 
                }).ToList();
            }

            return Ok(new EmbeddingVisualizationResponse(
                Points: points,
                CollectionName: collectionName,
                EmbeddingModel: embeddingModel,
                OriginalDimensions: dimensions
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating embedding visualization");
            return StatusCode(500, new { error = "An error occurred while generating visualization", details = ex.Message });
        }
    }

    /// <summary>
    /// Simple 2D projection using first two principal components (simplified).
    /// In production, use UMAP or t-SNE on the frontend for better visualization.
    /// </summary>
    private static (float X, float Y) SimpleProject(float[] embedding)
    {
        if (embedding.Length < 2)
        {
            return (0, 0);
        }

        // Simple projection: use weighted sum of different parts of the embedding
        // This is a placeholder - real visualization would use UMAP/t-SNE
        var third = embedding.Length / 3;
        
        float x = 0, y = 0;
        for (int i = 0; i < third; i++)
        {
            x += embedding[i];
        }
        for (int i = third; i < 2 * third; i++)
        {
            y += embedding[i];
        }
        
        x /= third;
        y /= third;
        
        return (x, y);
    }
}
