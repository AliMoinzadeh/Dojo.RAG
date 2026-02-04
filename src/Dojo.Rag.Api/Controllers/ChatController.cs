using Microsoft.AspNetCore.Mvc;
using Dojo.Rag.Api.Models;
using Dojo.Rag.Api.Services;

namespace Dojo.Rag.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IRagOrchestrator _ragOrchestrator;
    private readonly IDocumentIngestionService _ingestionService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IRagOrchestrator ragOrchestrator,
        IDocumentIngestionService ingestionService,
        ILogger<ChatController> logger)
    {
        _ragOrchestrator = ragOrchestrator;
        _ingestionService = ingestionService;
        _logger = logger;
    }

    /// <summary>
    /// Send a chat message and get a RAG-augmented response.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // Auto-ingest if needed when switching embedding models
            await _ingestionService.AutoIngestIfNeededAsync(cancellationToken);
            
            var response = await _ragOrchestrator.ChatAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            return StatusCode(500, new { error = "An error occurred while processing your request", details = ex.Message });
        }
    }
}
