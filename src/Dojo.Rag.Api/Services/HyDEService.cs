using Microsoft.Extensions.AI;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Service for generating hypothetical documents (HyDE) from a query.
/// </summary>
public interface IHyDEService
{
    Task<string> GenerateHypotheticalDocumentAsync(string query, CancellationToken cancellationToken = default);
}

public class HyDEService : IHyDEService
{
    private readonly IAIServiceFactory _aiServiceFactory;
    private readonly ILogger<HyDEService> _logger;

    public HyDEService(IAIServiceFactory aiServiceFactory, ILogger<HyDEService> logger)
    {
        _aiServiceFactory = aiServiceFactory;
        _logger = logger;
    }

    public async Task<string> GenerateHypotheticalDocumentAsync(string query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating HyDE document for query: {Query}", query);

        try
        {
            var chatClient = _aiServiceFactory.CreateChatClient();

            var systemPrompt = "Du erzeugst ein hypothetisches Dokument (HyDE) fuer eine Suche im Bereich Kaffee. " +
                               "Schreibe 2-4 Saetze, die eine plausible Antwort oder einen relevanten Dokumentauszug darstellen. " +
                               "Keine Aufzaehlungen, keine Erklaerungen, nur Fliesstext. Maximal 80 Woerter.";

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, systemPrompt),
                new(ChatRole.User, query)
            };

            var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
            var hypothetical = response.Text?.Trim() ?? query;

            _logger.LogInformation("HyDE generated for query: {Query}", query);

            return hypothetical;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "HyDE generation failed, using original query: {Query}", query);
            return query;
        }
    }
}
