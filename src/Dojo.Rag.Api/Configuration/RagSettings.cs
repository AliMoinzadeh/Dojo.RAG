namespace Dojo.Rag.Api.Configuration;

public class RagSettings
{
    public const string SectionName = "Rag";
    
    public int ChunkSize { get; set; } = 500;
    public int ChunkOverlap { get; set; } = 100;
    public int TopK { get; set; } = 5;
    public double MinRelevanceScore { get; set; } = 0.5;
}
