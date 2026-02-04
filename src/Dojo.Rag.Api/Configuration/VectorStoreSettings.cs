namespace Dojo.Rag.Api.Configuration;

public class VectorStoreSettings
{
    public const string SectionName = "VectorStore";
    
    public string Provider { get; set; } = "InMemory";
    public QdrantSettings Qdrant { get; set; } = new();
}

public class QdrantSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6334;
}
