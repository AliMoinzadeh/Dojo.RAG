using Microsoft.Extensions.Options;
using Dojo.Rag.Api.Configuration;
using Dojo.Rag.Api.Models;
using System.Text.RegularExpressions;

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

        if (_settings.UseSemanticChunking)
        {
            return ChunkBySemanticBoundaries(document, content);
        }

        return ChunkWithOverlap(document, content);
    }

    private IEnumerable<DocumentChunk> ChunkWithOverlap(SourceDocument document, string content)
    {
        var chunks = new List<DocumentChunk>();

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

    private IEnumerable<DocumentChunk> ChunkBySemanticBoundaries(SourceDocument document, string content)
    {
        var chunks = new List<DocumentChunk>();
        var targetSize = _settings.ChunkSize;
        var maxSize = (int)(targetSize * 1.3);

        var segments = ExtractSentenceSegments(content).ToList();
        if (segments.Count == 0)
        {
            return chunks;
        }

        int chunkIndex = 0;
        int currentStart = segments[0].StartIndex;
        int currentEnd = segments[0].EndIndex;

        for (int i = 1; i < segments.Count; i++)
        {
            var segment = segments[i];
            var currentLength = currentEnd - currentStart;
            var nextLength = segment.EndIndex - currentStart;

            if (nextLength <= maxSize || currentLength < targetSize)
            {
                currentEnd = segment.EndIndex;
                continue;
            }

            var chunk = CreateChunkFromRange(document, content, currentStart, currentEnd, chunkIndex++);
            if (chunk != null)
            {
                chunks.Add(chunk);
            }

            currentStart = segment.StartIndex;
            currentEnd = segment.EndIndex;
        }

        var finalChunk = CreateChunkFromRange(document, content, currentStart, currentEnd, chunkIndex);
        if (finalChunk != null)
        {
            chunks.Add(finalChunk);
        }

        _logger.LogInformation("Semantically chunked document {FileName} into {ChunkCount} chunks (target size: {ChunkSize})",
            document.FileName, chunks.Count, targetSize);

        return chunks;
    }

    private static IEnumerable<(int StartIndex, int EndIndex)> ExtractSentenceSegments(string content)
    {
        foreach (Match match in Regex.Matches(content, @"[^.!?\n]+[.!?]+|\S[^.!?\n]*$"))
        {
            var value = match.Value.Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            yield return (match.Index, match.Index + match.Length);
        }
    }

    private static DocumentChunk? CreateChunkFromRange(
        SourceDocument document,
        string content,
        int startIndex,
        int endIndex,
        int chunkIndex)
    {
        if (endIndex <= startIndex)
        {
            return null;
        }

        var rawChunk = content.Substring(startIndex, endIndex - startIndex);
        var trimmed = rawChunk.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return null;
        }

        var leadingTrim = rawChunk.Length - rawChunk.TrimStart().Length;
        var trailingTrim = rawChunk.Length - rawChunk.TrimEnd().Length;
        var adjustedStart = startIndex + leadingTrim;
        var adjustedEnd = endIndex - trailingTrim;

        return DocumentChunkFactory.Create(
            content: trimmed,
            sourceDocument: document.Id,
            sourceFileName: document.FileName,
            chunkIndex: chunkIndex,
            startCharIndex: adjustedStart,
            endCharIndex: adjustedEnd
        );
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
