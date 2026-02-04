using Microsoft.Extensions.Options;
using Dojo.Rag.Api.Configuration;
using Dojo.Rag.Api.Models;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for chunking documents into smaller pieces for embedding.
/// Implements overlapping chunk strategy for better context preservation.
/// </summary>
public interface IDocumentChunker
{
    IEnumerable<DocumentChunk> ChunkDocument(SourceDocument document);
}

public class DocumentChunker : IDocumentChunker
{
    private readonly RagSettings _settings;
    private readonly ILogger<DocumentChunker> _logger;

    public DocumentChunker(IOptions<RagSettings> settings, ILogger<DocumentChunker> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public IEnumerable<DocumentChunk> ChunkDocument(SourceDocument document)
    {
        var content = document.Content;
        var chunks = new List<DocumentChunk>();
        
        if (string.IsNullOrWhiteSpace(content))
        {
            _logger.LogWarning("Document {FileName} has no content to chunk", document.FileName);
            return chunks;
        }

        var chunkSize = _settings.ChunkSize;
        var overlap = _settings.ChunkOverlap;
        var step = chunkSize - overlap;
        
        if (step <= 0)
        {
            step = chunkSize / 2;
        }

        int chunkIndex = 0;
        int position = 0;
        
        while (position < content.Length)
        {
            var endPosition = Math.Min(position + chunkSize, content.Length);
            var chunkText = content.Substring(position, endPosition - position);
            
            // Try to break at sentence/paragraph boundaries
            if (endPosition < content.Length)
            {
                chunkText = AdjustChunkBoundary(chunkText, content, position, chunkSize);
            }
            
            if (!string.IsNullOrWhiteSpace(chunkText))
            {
                var chunk = DocumentChunkFactory.Create(
                    content: chunkText.Trim(),
                    sourceDocument: document.Id,
                    sourceFileName: document.FileName,
                    chunkIndex: chunkIndex,
                    startCharIndex: position,
                    endCharIndex: position + chunkText.Length
                );
                
                chunks.Add(chunk);
                chunkIndex++;
            }
            
            position += step;
        }
        
        _logger.LogInformation("Chunked document {FileName} into {ChunkCount} chunks (size: {ChunkSize}, overlap: {Overlap})",
            document.FileName, chunks.Count, chunkSize, overlap);
        
        return chunks;
    }

    private string AdjustChunkBoundary(string chunk, string fullContent, int startPosition, int maxLength)
    {
        // Try to find a good breaking point (sentence end, paragraph, etc.)
        var breakPoints = new[] { "\n\n", ".\n", ". ", "!\n", "! ", "?\n", "? ", "\n" };
        
        foreach (var breakPoint in breakPoints)
        {
            var lastBreak = chunk.LastIndexOf(breakPoint, StringComparison.Ordinal);
            if (lastBreak > maxLength / 2) // Don't break if it would make chunk too small
            {
                return chunk.Substring(0, lastBreak + breakPoint.Length);
            }
        }
        
        return chunk;
    }
}
