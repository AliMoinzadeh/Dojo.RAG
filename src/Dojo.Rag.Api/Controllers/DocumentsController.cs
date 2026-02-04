using Microsoft.AspNetCore.Mvc;
using Dojo.Rag.Api.Models;
using Dojo.Rag.Api.Services;

namespace Dojo.Rag.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentIngestionService _ingestionService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IDocumentIngestionService ingestionService,
        ILogger<DocumentsController> logger)
    {
        _ingestionService = ingestionService;
        _logger = logger;
    }

    /// <summary>
    /// Ingest a new document into the RAG system.
    /// </summary>
    [HttpPost("ingest")]
    public async Task<ActionResult<IngestResponse>> Ingest([FromBody] IngestRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.FileName))
            {
                return BadRequest(new { error = "FileName is required" });
            }
            
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { error = "Content is required" });
            }
            
            var response = await _ingestionService.IngestDocumentAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting document {FileName}", request.FileName);
            return StatusCode(500, new { error = "An error occurred while ingesting the document", details = ex.Message });
        }
    }

    /// <summary>
    /// Get all source documents (not chunked).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SourceDocument>>> GetDocuments(CancellationToken cancellationToken)
    {
        try
        {
            var documents = await _ingestionService.GetSourceDocumentsAsync(cancellationToken);
            return Ok(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents");
            return StatusCode(500, new { error = "An error occurred while getting documents", details = ex.Message });
        }
    }

    /// <summary>
    /// Re-ingest a document (useful after switching embedding models).
    /// </summary>
    [HttpPost("{documentId}/reingest")]
    public async Task<ActionResult<IngestResponse>> ReIngest(string documentId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _ingestionService.ReIngestDocumentAsync(documentId, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error re-ingesting document {DocumentId}", documentId);
            return StatusCode(500, new { error = "An error occurred while re-ingesting the document", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete a specific document by ID.
    /// </summary>
    [HttpDelete("{documentId}")]
    public async Task<ActionResult> DeleteDocument(string documentId, CancellationToken cancellationToken)
    {
        try
        {
            await _ingestionService.DeleteDocumentAsync(documentId, cancellationToken);
            return Ok(new { message = $"Document {documentId} deleted successfully" });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", documentId);
            return StatusCode(500, new { error = "An error occurred while deleting the document", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete all documents and clear all vector collections.
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult> DeleteAllDocuments(CancellationToken cancellationToken)
    {
        try
        {
            await _ingestionService.DeleteAllDocumentsAsync(cancellationToken);
            return Ok(new { message = "All documents and collections deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all documents");
            return StatusCode(500, new { error = "An error occurred while deleting all documents", details = ex.Message });
        }
    }
}
