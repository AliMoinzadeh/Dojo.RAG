namespace Dojo.Rag.Api.Configuration;

public class AISettings
{
    public const string SectionName = "AI";
    
    /// <summary>
    /// AI provider: "OpenAI", "Ollama", or "LMStudio"
    /// </summary>
    public string Provider { get; set; } = "Ollama";
    public OpenAISettings OpenAI { get; set; } = new();
    public OllamaSettings Ollama { get; set; } = new();
    public LMStudioSettings LMStudio { get; set; } = new();
    
    public string GetActiveEmbeddingModel() => Provider switch
    {
        "OpenAI" => OpenAI.EmbeddingModel,
        "Ollama" => Ollama.EmbeddingModel,
        "LMStudio" => LMStudio.EmbeddingModel,
        _ => throw new ArgumentException($"Unknown provider: {Provider}")
    };
    
    public int GetActiveEmbeddingDimensions() => Provider switch
    {
        "OpenAI" => OpenAI.EmbeddingDimensions,
        "Ollama" => Ollama.EmbeddingDimensions,
        "LMStudio" => LMStudio.EmbeddingDimensions,
        _ => throw new ArgumentException($"Unknown provider: {Provider}")
    };
}

public class OpenAISettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string ChatModel { get; set; } = "gpt-4o-mini";
    public string EmbeddingModel { get; set; } = "text-embedding-3-small";
    public int EmbeddingDimensions { get; set; } = 1536;
}

public class OllamaSettings
{
    public string Endpoint { get; set; } = "http://localhost:11434";
    public string ChatModel { get; set; } = "llama3.2";
    public string EmbeddingModel { get; set; } = "all-minilm";
    public int EmbeddingDimensions { get; set; } = 384;
}

/// <summary>
/// LM Studio settings - uses OpenAI-compatible API
/// </summary>
public class LMStudioSettings
{
    public string Endpoint { get; set; } = "http://localhost:1234/v1";
    public string ChatModel { get; set; } = "llama-3.2-1b-instruct";
    public string EmbeddingModel { get; set; } = "text-embedding-qwen3-embedding-0.6b";
    public int EmbeddingDimensions { get; set; } = 1024;
}
