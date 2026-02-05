using Microsoft.Extensions.AI;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for expanding search queries using LLM to generate synonyms and related terms.
/// This helps find documents that use different terminology for the same concept.
/// </summary>
public interface IQueryExpansionService
{
    /// <summary>
    /// Expands the query with synonyms and related terms using an LLM.
    /// </summary>
    Task<string> ExpandQueryAsync(string query, CancellationToken cancellationToken = default);
}

public class QueryExpansionService : IQueryExpansionService
{
    private readonly IAIServiceFactory _aiServiceFactory;
    private readonly ILogger<QueryExpansionService> _logger;

    public QueryExpansionService(
        IAIServiceFactory aiServiceFactory,
        ILogger<QueryExpansionService> logger)
    {
        _aiServiceFactory = aiServiceFactory;
        _logger = logger;
    }

    public async Task<string> ExpandQueryAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Expanding query: {Query}", query);

        try
        {
            var chatClient = _aiServiceFactory.CreateChatClient();

            var systemPrompt = @"Du bist ein Assistent für Suchanfragen-Erweiterung im Bereich Kaffee und Kaffeezubereitung.
Deine Aufgabe ist es, die Suchanfrage des Benutzers mit Synonymen und verwandten Begriffen zu erweitern.

Regeln:
1. Behalte die originalen Suchbegriffe
2. Füge Synonyme und Fachbegriffe hinzu
3. Füge verwandte Konzepte hinzu
4. Antworte NUR mit der erweiterten Suchanfrage, keine Erklärungen
5. Trenne Begriffe mit Leerzeichen
6. Maximal 15 Wörter insgesamt

Beispiele:
- 'Kaffee Schaum' → 'Kaffee Schaum Crema Espresso Milchschaum'
- 'heißes Getränk' → 'heißes Getränk Kaffee Espresso Cappuccino warm Temperatur'
- 'Java' → 'Java Kaffee Coffee Bohnen Arabica'";

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, systemPrompt),
                new(ChatRole.User, query)
            };

            var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
            var expandedQuery = response.Text?.Trim() ?? query;

            _logger.LogInformation("Expanded query: {OriginalQuery} → {ExpandedQuery}", query, expandedQuery);

            return expandedQuery;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to expand query, using original: {Query}", query);
            return query;
        }
    }
}
