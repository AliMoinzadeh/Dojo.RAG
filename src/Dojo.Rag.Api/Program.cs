using Microsoft.Extensions.AI;
using Dojo.Rag.Api.Configuration;
using Dojo.Rag.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<AISettings>(builder.Configuration.GetSection(AISettings.SectionName));
builder.Services.Configure<VectorStoreSettings>(builder.Configuration.GetSection(VectorStoreSettings.SectionName));
builder.Services.Configure<RagSettings>(builder.Configuration.GetSection(RagSettings.SectionName));

// AI Services
builder.Services.AddSingleton<IAIServiceFactory, AIServiceFactory>();
builder.Services.AddSingleton<IChatClient>(sp => sp.GetRequiredService<IAIServiceFactory>().CreateChatClient());
builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp => 
    sp.GetRequiredService<IAIServiceFactory>().CreateEmbeddingGenerator());

// RAG Services
builder.Services.AddSingleton<IVectorStoreManager, VectorStoreManager>();
builder.Services.AddSingleton<IDocumentChunker, DocumentChunker>();
builder.Services.AddSingleton<IEmbeddingService, EmbeddingService>();
builder.Services.AddSingleton<IVectorSearchService, VectorSearchService>();
builder.Services.AddSingleton<IDocumentIngestionService, DocumentIngestionService>();
builder.Services.AddSingleton<IRagOrchestrator, RagOrchestrator>();
builder.Services.AddSingleton<ITokenCounterService, TokenCounterService>();

// Vector Search Demo Services
builder.Services.AddSingleton<IHybridSearchService, HybridSearchService>();
builder.Services.AddSingleton<IQueryExpansionService, QueryExpansionService>();

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RAG Demo API", Version = "v1" });
});

// CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowReactApp");

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RAG Demo API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

app.Run();
