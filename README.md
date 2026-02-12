# RAG Demo - Retrieval-Augmented Generation

An educational demo project for teaching RAG (Retrieval-Augmented Generation) concepts using .NET 9 and React.

## 🎯 Purpose

This project demonstrates:
- Document chunking strategies
- Vector embeddings and similarity search
- RAG pipeline orchestration
- Provider switching (OpenAI/Ollama)
- Multi-model embedding support with separate collections

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────┐
│                  React Frontend                 │
│  Chat UI │ Config Panel │ Pipeline Visualizer   │
└─────────────────────┬───────────────────────────┘
                      │
┌─────────────────────▼───────────────────────────┐
│               .NET 9 Web API                    │
│  RAG Orchestrator → Embedder → Vector Store     │
└─────────────────────┬───────────────────────────┘
           ┌──────────┴──────────┐
     ┌─────▼─────┐        ┌──────▼──────┐
     │  Ollama   │        │   Qdrant    │
     └───────────┘        └─────────────┘
```

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Docker](https://www.docker.com/) (for Qdrant)
- [Ollama](https://ollama.ai/) (for local models)

### Ollama Models

```bash
# Pull required models
ollama pull llama3.2
ollama pull nomic-embed-text
```

## 🚀 Quick Start

### 1. Start Qdrant (Vector Database)

```bash
docker-compose up -d
```

Dashboard available at: http://localhost:6333/dashboard

### 2. Start the API

```bash
cd src/Dojo.Rag.Api
dotnet run
```

API available at: http://localhost:5000
Swagger UI: http://localhost:5000/swagger

### 3. Start the Frontend

```bash
cd src/Dojo.Rag.UI
npm install
npm run dev
```

Frontend available at: http://localhost:5173

## ⚙️ Configuration

### AI Provider Settings

Edit `src/Dojo.Rag.Api/appsettings.json`:

```json
{
  "AI": {
    "Provider": "Ollama",  // or "OpenAI" or "LMStudio"
    "Ollama": {
      "Endpoint": "http://localhost:11434",
      "ChatModel": "llama3.2",
      "EmbeddingModel": "nomic-embed-text"
    },
    "OpenAI": {
      "ApiKey": "your-api-key",
      "ChatModel": "gpt-4o-mini",
      "EmbeddingModel": "text-embedding-3-small"
    },
    "LMStudio": {
      "Endpoint": "http://localhost:1234/v1",
      "ChatModel": "lmstudio-community/Meta-Llama-3.1-8B-Instruct-GGUF",
      "EmbeddingModel": "nomic-ai/nomic-embed-text-v1.5-GGUF"
    }
  }
}
```

### Vector Store Settings

```json
{
  "VectorStore": {
    "Provider": "InMemory",  // or "Qdrant"
    "Qdrant": {
      "Endpoint": "http://localhost:6333"
    }
  }
}
```

### RAG Parameters

```json
{
  "Rag": {
    "ChunkSize": 500,
    "ChunkOverlap": 50,
    "TopK": 5,
    "MinRelevanceScore": 0.7
  }
}
```

## 📚 API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/chat` | POST | Send a chat message with RAG |
| `/api/documents` | POST | Upload a document |
| `/api/documents` | GET | List all documents |
| `/api/collections` | GET | List vector collections |
| `/api/collections/{name}` | DELETE | Delete a collection |
| `/api/embeddings/models` | GET | Get current embedding model |

## 📁 Project Structure

```
dojo.rag/
├── src/
│   ├── Dojo.Rag.Api/           # .NET Web API
│   │   ├── Configuration/       # Settings classes
│   │   ├── Controllers/         # API endpoints
│   │   ├── Models/              # Data models
│   │   └── Services/            # Business logic
│   └── Dojo.Rag.UI/            # React frontend
│       └── src/
│           ├── components/      # React components
│           ├── lib/             # API client
│           └── types/           # TypeScript types
├── docs/
│   ├── presentation.md         # Marp slides
│   └── sample-documents/       # Demo content
└── docker-compose.yml          # Qdrant setup
```

## 🎓 Educational Features

### Pipeline Visualization
Watch the RAG pipeline in action with step-by-step visualization.

### Relevance Scores
See similarity scores for retrieved chunks.

### Context Window Gauge
Monitor token usage as context grows.

### Multi-Model Support
Switch between embedding models and see how collections are managed.

## 📊 Presentation

Presentations are authored in Markdown (for example `docs/presentation.de.md`).

Generate HTML (including Mermaid diagrams) with:

```powershell
.\scripts\build-presentation.ps1
```

Custom input/output:

```powershell
.\scripts\build-presentation.ps1 -InputPath docs/presentation.md -OutputPath docs/presentation.html
```

The script uses `npx` to run `@mermaid-js/mermaid-cli` and `@marp-team/marp-cli`, so no global installs are required.

## 🧪 Sample Queries

After uploading the sample documents, try:

- "What's the difference between espresso and pour-over?"
- "How long should I steep French press coffee?"
- "What makes Ethiopian coffee unique?"
- "Explain the cold brew process"

## 🔧 Troubleshooting

### Ollama Connection Issues
```bash
# Ensure Ollama is running
ollama serve

# Check available models
ollama list
```

### Qdrant Connection Issues
```bash
# Check container status
docker-compose ps

# View logs
docker-compose logs qdrant
```

### CORS Errors
The API is configured for `localhost:5173` and `localhost:3000`. Update `Program.cs` if using different ports.

## 📄 License

MIT License - Use freely for learning and teaching!
