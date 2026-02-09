namespace Dojo.Rag.Api.Models;

/// <summary>
/// Models for the Vector Search Demo feature.
/// </summary>
/// 
// Request/Response for demo sentences
public record DemoSentence(
    string Id,
    string Text,
    string Category,
    List<string> Tags
);

public record DemoScenario(
    string Name,
    string Query,
    string ExpectedMatch,
    string Explanation
);

public record DemoSentencesResponse(
    string Description,
    List<DemoSentence> Sentences,
    List<DemoScenario> DemoScenarios
);

// Search enhancement options
public record SearchEnhancements(
    bool UseHybridSearch = false,
    bool UseQueryExpansion = false,
    bool UseReranking = false
);

// Search request/response
public record VectorSearchDemoRequest(
    string Query,
    SearchEnhancements? Enhancements = null,
    int TopK = 5
);

public record SearchResultItem(
    string Id,
    string Text,
    string Category,
    double Score,
    List<string> MatchedKeywords // For hybrid search visualization
);

public record SearchResultSet(
    List<SearchResultItem> Results,
    long SearchTimeMs,
    string? ExpandedQuery = null // Shows what query expansion produced
);

public record VectorSearchDemoResponse(
    SearchResultSet StandardResults,
    SearchResultSet? EnhancedResults,
    string OriginalQuery,
    SearchEnhancements AppliedEnhancements
);

// Initialize demo collection
public record InitializeDemoResponse(
    bool Success,
    int SentencesEmbedded,
    string CollectionName,
    string Message
);
