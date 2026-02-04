using Microsoft.AspNetCore.Mvc;
using Dojo.Rag.Api.Models;
using Dojo.Rag.Api.Services;

namespace Dojo.Rag.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
    private readonly IVectorStoreManager _vectorStoreManager;
    private readonly IAIServiceFactory _aiServiceFactory;
    private readonly ILogger<CollectionsController> _logger;

    public CollectionsController(
        IVectorStoreManager vectorStoreManager,
        IAIServiceFactory aiServiceFactory,
        ILogger<CollectionsController> logger)
    {
        _vectorStoreManager = vectorStoreManager;
        _aiServiceFactory = aiServiceFactory;
        _logger = logger;
    }

    /// <summary>
    /// Get all vector store collections.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CollectionInfo>>> GetCollections(CancellationToken cancellationToken)
    {
        try
        {
            var collections = await _vectorStoreManager.ListCollectionsAsync(cancellationToken);
            return Ok(collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting collections");
            return StatusCode(500, new { error = "An error occurred while getting collections", details = ex.Message });
        }
    }

    /// <summary>
    /// Get current active configuration.
    /// </summary>
    [HttpGet("active")]
    public ActionResult<object> GetActiveConfig()
    {
        try
        {
            return Ok(new
            {
                Provider = _aiServiceFactory.GetActiveProvider(),
                ChatModel = _aiServiceFactory.GetActiveChatModel(),
                EmbeddingModel = _aiServiceFactory.GetActiveEmbeddingModel(),
                EmbeddingDimensions = _aiServiceFactory.GetActiveEmbeddingDimensions(),
                CollectionName = _vectorStoreManager.GetCollectionName(_aiServiceFactory.GetActiveEmbeddingModel())
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active config");
            return StatusCode(500, new { error = "An error occurred while getting active configuration", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete a specific collection.
    /// </summary>
    [HttpDelete("{collectionName}")]
    public async Task<ActionResult> DeleteCollection(string collectionName, CancellationToken cancellationToken)
    {
        try
        {
            await _vectorStoreManager.DeleteCollectionAsync(collectionName, cancellationToken);
            return Ok(new { message = $"Collection {collectionName} deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting collection {CollectionName}", collectionName);
            return StatusCode(500, new { error = "An error occurred while deleting the collection", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete all collections.
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult> DeleteAllCollections(CancellationToken cancellationToken)
    {
        try
        {
            await _vectorStoreManager.DeleteAllCollectionsAsync(cancellationToken);
            return Ok(new { message = "All collections deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all collections");
            return StatusCode(500, new { error = "An error occurred while deleting all collections", details = ex.Message });
        }
    }
}
