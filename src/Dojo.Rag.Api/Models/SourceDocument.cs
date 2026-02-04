namespace Dojo.Rag.Api.Models;

/// <summary>
/// Represents a source document before processing
/// </summary>
public class SourceDocument
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
