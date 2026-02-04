using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using Dojo.Rag.Api.Configuration;

namespace Dojo.Rag.Api.Services;

/// <summary>
/// Factory for creating AI service instances (Chat and Embedding clients) based on configuration.
/// Supports switching between OpenAI and Ollama providers via appsettings.
/// </summary>
public interface IAIServiceFactory
{
    IChatClient CreateChatClient();
    IEmbeddingGenerator<string, Embedding<float>> CreateEmbeddingGenerator();
    string GetActiveProvider();
    string GetActiveChatModel();
    string GetActiveEmbeddingModel();
    int GetActiveEmbeddingDimensions();
}

public class AIServiceFactory : IAIServiceFactory
{
    private readonly AISettings _settings;
    private readonly ILogger<AIServiceFactory> _logger;

    public AIServiceFactory(IOptions<AISettings> settings, ILogger<AIServiceFactory> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public IChatClient CreateChatClient()
    {
        _logger.LogInformation("Creating chat client for provider: {Provider}", _settings.Provider);
        
        return _settings.Provider switch
        {
            "OpenAI" => CreateOpenAIChatClient(),
            "Ollama" => CreateOllamaChatClient(),
            "LMStudio" => CreateLMStudioChatClient(),
            _ => throw new ArgumentException($"Unknown AI provider: {_settings.Provider}")
        };
    }

    public IEmbeddingGenerator<string, Embedding<float>> CreateEmbeddingGenerator()
    {
        _logger.LogInformation("Creating embedding generator for provider: {Provider}, model: {Model}", 
            _settings.Provider, GetActiveEmbeddingModel());
        
        return _settings.Provider switch
        {
            "OpenAI" => CreateOpenAIEmbeddingGenerator(),
            "Ollama" => CreateOllamaEmbeddingGenerator(),
            "LMStudio" => CreateLMStudioEmbeddingGenerator(),
            _ => throw new ArgumentException($"Unknown AI provider: {_settings.Provider}")
        };
    }

    public string GetActiveProvider() => _settings.Provider;
    
    public string GetActiveChatModel() => _settings.Provider switch
    {
        "OpenAI" => _settings.OpenAI.ChatModel,
        "Ollama" => _settings.Ollama.ChatModel,
        "LMStudio" => _settings.LMStudio.ChatModel,
        _ => throw new ArgumentException($"Unknown provider: {_settings.Provider}")
    };

    public string GetActiveEmbeddingModel() => _settings.GetActiveEmbeddingModel();
    
    public int GetActiveEmbeddingDimensions() => _settings.GetActiveEmbeddingDimensions();

    private IChatClient CreateOpenAIChatClient()
    {
        if (string.IsNullOrEmpty(_settings.OpenAI.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured. Set AI:OpenAI:ApiKey in appsettings.json");
        }

        var client = new OpenAIClient(_settings.OpenAI.ApiKey);
        return client.GetChatClient(_settings.OpenAI.ChatModel).AsIChatClient();
    }

    private IEmbeddingGenerator<string, Embedding<float>> CreateOpenAIEmbeddingGenerator()
    {
        if (string.IsNullOrEmpty(_settings.OpenAI.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured. Set AI:OpenAI:ApiKey in appsettings.json");
        }

        var client = new OpenAIClient(_settings.OpenAI.ApiKey);
        return client.GetEmbeddingClient(_settings.OpenAI.EmbeddingModel).AsIEmbeddingGenerator();
    }

    private IChatClient CreateOllamaChatClient()
    {
        var endpoint = new Uri(_settings.Ollama.Endpoint);
        return new OllamaChatClient(endpoint, _settings.Ollama.ChatModel);
    }

    private IEmbeddingGenerator<string, Embedding<float>> CreateOllamaEmbeddingGenerator()
    {
        var endpoint = new Uri(_settings.Ollama.Endpoint);
        return new OllamaEmbeddingGenerator(endpoint, _settings.Ollama.EmbeddingModel);
    }

    private IChatClient CreateLMStudioChatClient()
    {
        // LM Studio uses OpenAI-compatible API
        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(_settings.LMStudio.Endpoint)
        };
        // LM Studio doesn't require a real API key, but the SDK requires one
        var client = new OpenAIClient(new System.ClientModel.ApiKeyCredential("lm-studio"), options);
        return client.GetChatClient(_settings.LMStudio.ChatModel).AsIChatClient();
    }

    private IEmbeddingGenerator<string, Embedding<float>> CreateLMStudioEmbeddingGenerator()
    {
        // LM Studio uses OpenAI-compatible API
        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(_settings.LMStudio.Endpoint)
        };
        var client = new OpenAIClient(new System.ClientModel.ApiKeyCredential("lm-studio"), options);
        return client.GetEmbeddingClient(_settings.LMStudio.EmbeddingModel).AsIEmbeddingGenerator();
    }
}
