using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Dojo.Rag.Api.Models;
using Dojo.Rag.Api.Services;

namespace Dojo.Rag.Api.Controllers;

/// <summary>
/// Controller for the Vector Search Demo feature.
/// Demonstrates vector search limitations and improvement techniques.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VectorSearchDemoController : ControllerBase
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IHybridSearchService _hybridSearchService;
    private readonly IQueryExpansionService _queryExpansionService;
    private readonly IRerankingService _rerankingService;
    private readonly IMultiVectorSearchService _multiVectorSearchService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<VectorSearchDemoController> _logger;

    // In-memory cache for demo sentences and their embeddings
    private static DemoSentencesResponse? _cachedSentences;
    private static readonly ConcurrentDictionary<string, float[]> _sentenceEmbeddings = new();
    private static readonly ConcurrentDictionary<string, float[]> _tagEmbeddings = new();
    private static string? _embeddingModelUsed;

    public VectorSearchDemoController(
        IEmbeddingService embeddingService,
        IHybridSearchService hybridSearchService,
        IQueryExpansionService queryExpansionService,
        IRerankingService rerankingService,
        IMultiVectorSearchService multiVectorSearchService,
        IWebHostEnvironment environment,
        ILogger<VectorSearchDemoController> logger)
    {
        _embeddingService = embeddingService;
        _hybridSearchService = hybridSearchService;
        _queryExpansionService = queryExpansionService;
        _rerankingService = rerankingService;
        _multiVectorSearchService = multiVectorSearchService;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Get demo sentences from the JSON file.
    /// </summary>
    [HttpGet("sentences")]
    public async Task<ActionResult<DemoSentencesResponse>> GetSentences()
    {
        try
        {
            var sentences = await LoadSentencesAsync();
            return Ok(sentences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading demo sentences");
            return StatusCode(500, new { error = "Failed to load demo sentences", details = ex.Message });
        }
    }

    /// <summary>
    /// Initialize demo collection by embedding all sentences.
    /// Call this before performing searches.
    /// </summary>
    [HttpPost("initialize")]
    public async Task<ActionResult<InitializeDemoResponse>> InitializeDemo(CancellationToken cancellationToken)
    {
        try
        {
            var sentences = await LoadSentencesAsync();
            
            _logger.LogInformation("Initializing demo with {Count} sentences", sentences.Sentences.Count);

            // Clear existing embeddings if model changed or not initialized
            _sentenceEmbeddings.Clear();
            _tagEmbeddings.Clear();

            // Generate embeddings for all sentences
            var texts = sentences.Sentences.Select(s => s.Text).ToList();
            var embeddings = await _embeddingService.GenerateEmbeddingsAsync(texts, cancellationToken);

            var tagTexts = sentences.Sentences
                .Select(s => string.IsNullOrWhiteSpace(string.Join(" ", s.Tags)) ? s.Text : string.Join(" ", s.Tags))
                .ToList();
            var tagEmbeddings = await _embeddingService.GenerateEmbeddingsAsync(tagTexts, cancellationToken);

            for (int i = 0; i < sentences.Sentences.Count; i++)
            {
                var embeddingArray = embeddings[i].ToArray();
                _sentenceEmbeddings[sentences.Sentences[i].Id] = embeddingArray;

                var tagEmbeddingArray = tagEmbeddings[i].ToArray();
                _tagEmbeddings[sentences.Sentences[i].Id] = tagEmbeddingArray;
            }

            _embeddingModelUsed = "current"; // Track which model was used

            return Ok(new InitializeDemoResponse(
                Success: true,
                SentencesEmbedded: sentences.Sentences.Count,
                CollectionName: "demo-sentences",
                Message: $"Successfully embedded {sentences.Sentences.Count} demo sentences"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing demo");
            return StatusCode(500, new { error = "Failed to initialize demo", details = ex.Message });
        }
    }

    /// <summary>
    /// Perform vector search with optional enhancements.
    /// Returns side-by-side comparison of standard vs enhanced results.
    /// </summary>
    [HttpPost("search")]
    public async Task<ActionResult<VectorSearchDemoResponse>> Search(
        [FromBody] VectorSearchDemoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest(new { error = "Query cannot be empty" });
            }

            // Ensure demo is initialized
            if (_sentenceEmbeddings.IsEmpty)
            {
                return BadRequest(new { error = "Demo not initialized. Call POST /api/vectorsearchdemo/initialize first." });
            }

            var sentences = await LoadSentencesAsync();
            var enhancements = request.Enhancements ?? new SearchEnhancements();
            var minScore = request.MinScore;

            // Standard vector search
            var standardResults = await PerformVectorSearchAsync(
                request.Query, 
                sentences.Sentences, 
                request.TopK, 
                minScore,
                cancellationToken);

            // Enhanced search if requested
            SearchResultSet? enhancedResults = null;
                if (enhancements.UseHybridSearch || enhancements.UseQueryExpansion || enhancements.UseReranking || enhancements.UseMultiVectorSearch)
            {
                enhancedResults = await PerformEnhancedSearchAsync(
                    request.Query,
                    sentences.Sentences,
                    request.TopK,
                    minScore,
                    enhancements,
                    cancellationToken);
            }

            return Ok(new VectorSearchDemoResponse(
                StandardResults: standardResults,
                EnhancedResults: enhancedResults,
                OriginalQuery: request.Query,
                AppliedEnhancements: enhancements
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing search");
            return StatusCode(500, new { error = "Search failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Check if demo is initialized and ready.
    /// </summary>
    [HttpGet("status")]
    public ActionResult<object> GetStatus()
    {
        return Ok(new
        {
            isInitialized = !_sentenceEmbeddings.IsEmpty,
            embeddedSentenceCount = _sentenceEmbeddings.Count,
            embeddingModel = _embeddingModelUsed
        });
    }

    private async Task<SearchResultSet> PerformVectorSearchAsync(
        string query,
        List<DemoSentence> sentences,
        int topK,
        double minScore,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        // Generate query embedding
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(query, cancellationToken);

        // Calculate cosine similarity with all sentence embeddings
        var scores = new List<(DemoSentence Sentence, double Score)>();

        foreach (var sentence in sentences)
        {
            if (_sentenceEmbeddings.TryGetValue(sentence.Id, out var embedding))
            {
                var score = CosineSimilarity(queryEmbedding.Span, embedding);
                scores.Add((sentence, score));
            }
        }

        var results = scores
            .Where(s => s.Score >= minScore)
            .OrderByDescending(s => s.Score)
            .Take(topK)
            .Select(s => new SearchResultItem(
                s.Sentence.Id,
                s.Sentence.Text,
                s.Sentence.Category,
                Math.Round(s.Score, 4),
                new List<string>() // No keyword matches for pure vector search
            ))
            .ToList();

        stopwatch.Stop();

        return new SearchResultSet(results, stopwatch.ElapsedMilliseconds);
    }

    private async Task<SearchResultSet> PerformEnhancedSearchAsync(
        string query,
        List<DemoSentence> sentences,
        int topK,
        double minScore,
        SearchEnhancements enhancements,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        string? expandedQuery = null;
        string searchQuery = query;

        // Apply query expansion if enabled
        if (enhancements.UseQueryExpansion)
        {
            expandedQuery = await _queryExpansionService.ExpandQueryAsync(query, cancellationToken);
            searchQuery = expandedQuery;
        }

        // Generate embedding for (possibly expanded) query
        var queryEmbedding = await _embeddingService.GenerateEmbeddingAsync(searchQuery, cancellationToken);

        List<SearchResultItem> results;

        if (enhancements.UseHybridSearch)
        {
            // Use hybrid search service
            results = await _hybridSearchService.SearchAsync(
                searchQuery,
                sentences,
                _sentenceEmbeddings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                queryEmbedding,
                topK);
        }
        else if (enhancements.UseMultiVectorSearch)
        {
            results = await _multiVectorSearchService.SearchAsync(
                sentences,
                _sentenceEmbeddings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                _tagEmbeddings.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                queryEmbedding,
                topK);
        }
        else
        {
            // Pure vector search with expanded query
            var scores = new List<(DemoSentence Sentence, double Score)>();

            foreach (var sentence in sentences)
            {
                if (_sentenceEmbeddings.TryGetValue(sentence.Id, out var embedding))
                {
                    var score = CosineSimilarity(queryEmbedding.Span, embedding);
                    scores.Add((sentence, score));
                }
            }

            results = scores
                .Where(s => s.Score >= minScore)
                .OrderByDescending(s => s.Score)
                .Take(topK)
                .Select(s => new SearchResultItem(
                    s.Sentence.Id,
                    s.Sentence.Text,
                    s.Sentence.Category,
                    Math.Round(s.Score, 4),
                    new List<string>()
                ))
                .ToList();
        }

        if (minScore > 0)
        {
            results = results
                .Where(result => result.Score >= minScore)
                .Take(topK)
                .ToList();
        }

            if (enhancements.UseReranking)
            {
                results = await _rerankingService.RerankAsync(query, results, topK, cancellationToken);
            }

        stopwatch.Stop();

        return new SearchResultSet(results, stopwatch.ElapsedMilliseconds, expandedQuery);
    }

    private static double CosineSimilarity(ReadOnlySpan<float> a, float[] b)
    {
        if (a.Length != b.Length) return 0;

        double dotProduct = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }

        var denominator = Math.Sqrt(normA) * Math.Sqrt(normB);
        return denominator == 0 ? 0 : dotProduct / denominator;
    }

    private async Task<DemoSentencesResponse> LoadSentencesAsync()
    {
        if (_cachedSentences != null)
        {
            return _cachedSentences;
        }

        // Load from docs/demo-sentences.json
        var jsonPath = Path.Combine(_environment.ContentRootPath, "..", "..", "docs", "demo-sentences.json");
        
        if (!System.IO.File.Exists(jsonPath))
        {
            throw new FileNotFoundException($"Demo sentences file not found at: {jsonPath}");
        }

        var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
        var jsonDoc = JsonDocument.Parse(jsonContent);
        var root = jsonDoc.RootElement;

        var sentences = new List<DemoSentence>();
        foreach (var item in root.GetProperty("sentences").EnumerateArray())
        {
            var tags = item.GetProperty("tags").EnumerateArray()
                .Select(t => t.GetString()!)
                .ToList();

            sentences.Add(new DemoSentence(
                item.GetProperty("id").GetString()!,
                item.GetProperty("text").GetString()!,
                item.GetProperty("category").GetString()!,
                tags
            ));
        }

        var scenarios = new List<DemoScenario>();
        foreach (var item in root.GetProperty("demoScenarios").EnumerateArray())
        {
            scenarios.Add(new DemoScenario(
                item.GetProperty("name").GetString()!,
                item.GetProperty("query").GetString()!,
                item.GetProperty("expectedMatch").GetString()!,
                item.GetProperty("explanation").GetString()!
            ));
        }

        _cachedSentences = new DemoSentencesResponse(
            root.GetProperty("description").GetString()!,
            sentences,
            scenarios
        );

        return _cachedSentences;
    }
}
