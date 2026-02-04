---
marp: true
theme: default
paginate: true
backgroundColor: #1e1e2e
color: #cdd6f4
style: |
  section {
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  }
  h1, h2, h3 {
    color: #89b4fa;
  }
  code {
    background-color: #313244;
    color: #a6e3a1;
  }
  a {
    color: #f5c2e7;
  }
  blockquote {
    border-left: 4px solid #89b4fa;
    padding-left: 1em;
    color: #bac2de;
  }
  table {
    font-size: 0.8em;
  }
  th {
    background-color: #313244;
  }
  .columns {
    display: flex;
    gap: 2em;
  }
  .col {
    flex: 1;
  }
---

# Retrieval-Augmented Generation (RAG)

## Building Intelligent Search with Vector Databases

![bg right:40% 80%](https://upload.wikimedia.org/wikipedia/commons/thumb/4/4b/Open_AI_Logo.svg/512px-Open_AI_Logo.svg.png)

---

# What is RAG?

**Retrieval-Augmented Generation** combines:

1. **Information Retrieval** - Finding relevant documents
2. **Language Models** - Generating human-like responses

> "Give the LLM the context it needs, rather than relying on what it memorized during training"

---

# Why RAG?

## Problems with Pure LLMs

- 🧠 **Knowledge Cutoff** - Training data has a date limit
- 🎭 **Hallucinations** - Confidently wrong answers
- 📦 **No Private Data** - Can't access your documents
- 💰 **Fine-tuning is Expensive** - Requires significant resources

## RAG Solves These

- ✅ Real-time knowledge updates
- ✅ Grounded in actual documents
- ✅ Works with your private data
- ✅ No model retraining needed

---

# RAG Architecture Overview

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   User      │    │   Vector    │    │   LLM       │
│   Query     │──▶│   Search    │───▶│   Generate  │
└─────────────┘    └─────────────┘    └─────────────┘
                          │
                          ▼
                   ┌─────────────┐
                   │  Retrieved  │
                   │  Documents  │
                   └─────────────┘
```

---

# The RAG Pipeline

## Step-by-Step Flow

1. **Ingest** - Load and chunk documents
2. **Embed** - Convert chunks to vectors
3. **Store** - Save in vector database
4. **Query** - User asks a question
5. **Retrieve** - Find similar chunks
6. **Augment** - Build prompt with context
7. **Generate** - LLM produces answer

---

# Step 1: Document Chunking

## Why Chunk?

- LLMs have **context window limits** (4K - 128K tokens)
- Smaller chunks = **more precise retrieval**
- Larger chunks = **more context preserved**

## Chunking Strategies

| Strategy | Description | Use Case |
|----------|-------------|----------|
| Fixed Size | Split by character count | Simple, fast |
| Sentence | Split at sentence boundaries | Preserves meaning |
| Semantic | Split by topic/meaning | Best quality |
| Recursive | Try multiple strategies | Balanced |

---

# Chunking: The Overlap Strategy

```
Document: "The quick brown fox jumps over the lazy dog."

Chunk Size: 20 chars | Overlap: 5 chars

Chunk 1: "The quick brown fox "
Chunk 2: " fox jumps over the "
Chunk 3: " the lazy dog."
         ↑↑↑↑↑
       Overlap preserves context
```

> **Rule of Thumb**: 10-20% overlap prevents context loss at chunk boundaries

---

# Step 2: Embeddings

## What Are Embeddings?

Embeddings are **dense vector representations** of text that capture semantic meaning.

```
"coffee brewing"  → [0.12, -0.45, 0.78, 0.23, ...]
"making espresso" → [0.14, -0.42, 0.81, 0.19, ...]  ← Similar!
"car engine"      → [-0.67, 0.32, -0.15, 0.89, ...] ← Different!
```

## Key Concept

Similar meanings → Similar vectors → Small distance

---

# Embedding Models

## Popular Choices

| Model | Dimensions | Provider |
|-------|------------|----------|
| text-embedding-3-small | 1536 | OpenAI |
| text-embedding-3-large | 3072 | OpenAI |
| nomic-embed-text | 768 | Ollama |
| all-MiniLM-L6-v2 | 384 | HuggingFace |

## Considerations

- **Dimensions** affect storage and search speed
- **Quality** varies by use case
- **Local vs API** trade-offs

---

# Step 3: Vector Databases

## Why Not Regular Databases?

```sql
-- This doesn't work for semantic search!
SELECT * FROM documents
WHERE content LIKE '%coffee brewing%'
```

## Vector DB Features

- 🔍 **Similarity Search** - Find nearest neighbors
- 📊 **ANN Algorithms** - Approximate nearest neighbor
- 🏎️ **Optimized Indexes** - HNSW, IVF, etc.
- 📈 **Scalability** - Billions of vectors

---

# Vector Database Options

## Popular Choices

<div class="columns">
<div class="col">

### Cloud/Managed
- Pinecone
- Weaviate Cloud
- Azure AI Search
- MongoDB Atlas

</div>
<div class="col">

### Self-Hosted
- **Qdrant** ⭐
- Milvus
- ChromaDB
- pgvector

</div>
</div>

> We use **Qdrant** - fast, feature-rich, great dashboard!

---

# Vector Similarity Metrics

## Distance Functions

| Metric | Formula | Use Case |
|--------|---------|----------|
| Cosine | 1 - cos(θ) | Most common, normalized |
| Euclidean | √Σ(a-b)² | Absolute distances |
| Dot Product | Σ(a×b) | Magnitude matters |

## Cosine Similarity

```
sim(A, B) = (A · B) / (||A|| × ||B||)

Range: -1 to 1 (1 = identical)
```

---

# Step 4: The Query Process

```
User: "How do I make espresso?"
         │
         ▼
┌──────────────────────────────────────┐
│  1. Embed the query                  │
│     → [0.12, -0.45, 0.78, ...]       │
├──────────────────────────────────────┤
│  2. Search vector database           │
│     → Top K similar chunks           │
├──────────────────────────────────────┤
│  3. Rank by relevance score          │
│     → Filter below threshold         │
└──────────────────────────────────────┘
```

---

# Step 5: Context Augmentation

## Building the Prompt

```
System: You are a helpful assistant. Use the following
context to answer questions. If the context doesn't
contain the answer, say "I don't know."

Context:
---
[Retrieved Chunk 1: Espresso basics...]
[Retrieved Chunk 2: Pressure and temperature...]
[Retrieved Chunk 3: Grind size importance...]
---

User: How do I make espresso?
```

---

# The RAG Prompt Template

```csharp
var prompt = $"""
You are a helpful assistant that answers questions
based on the provided context.

CONTEXT:
{string.Join("\n---\n", retrievedChunks)}

RULES:
- Only use information from the context above
- If unsure, say "I don't have enough information"
- Cite sources when possible

USER QUESTION: {userQuery}
""";
```

---

# Step 6: Generation

## The LLM Does Its Magic

- Reads the **augmented prompt**
- Generates a **grounded response**
- Can include **citations** to source documents

## Example Response

> "To make espresso, you need to use 9 bars of pressure
> and water at 90-96°C. The grind should be extra-fine,
> and extraction takes 25-30 seconds for a proper shot."
> 
> *Sources: espresso-basics.md*

---

# Key RAG Parameters

## Tuning for Quality

| Parameter | Description | Typical Value |
|-----------|-------------|---------------|
| **Chunk Size** | Characters per chunk | 500-1000 |
| **Overlap** | Shared chars between chunks | 50-200 |
| **Top K** | Number of chunks to retrieve | 3-10 |
| **Min Score** | Relevance threshold | 0.7-0.8 |

---

# Common RAG Challenges

## 1. Retrieval Quality

- Wrong chunks retrieved
- **Solution**: Better chunking, hybrid search

## 2. Lost in the Middle

- LLMs ignore middle context
- **Solution**: Rerank, limit chunks

## 3. Context Window Limits

- Too many chunks = truncation
- **Solution**: Summarization, selection

---

# Advanced Techniques

## Hybrid Search
Combine vector similarity + keyword matching (BM25)

## Query Expansion
Rewrite queries for better retrieval

## Hypothetical Document Embeddings (HyDE)
Generate hypothetical answer, embed that

## Reranking
Use cross-encoder to reorder results

---

# Multi-Model Considerations

## Embedding Model Collections

```
documents_nomic-embed-text     ← Ollama embeddings
documents_text-embedding-3-small ← OpenAI embeddings
```

> ⚠️ Different models = Different vector spaces
> 
> **Never mix embeddings from different models!**

---

# Demo Architecture

```
┌─────────────────────────────────────────────────┐
│                  React Frontend                 │
│  ┌─────────┐ ┌─────────┐ ┌─────────────────┐    │
│  │ Chat UI │ │ Config  │ │ Pipeline Viewer │    │
│  └─────────┘ └─────────┘ └─────────────────┘    │
└─────────────────────┬───────────────────────────┘
                      │ HTTP/REST
┌─────────────────────▼───────────────────────────┐
│               .NET 9 Web API                    │
│  ┌──────────────────────────────────────────┐   │
│  │           RAG Orchestrator               │   │
│  │  ┌─────────┐ ┌────────┐ ┌────────────┐   │   │
│  │  │ Chunker │ │Embedder│ │ VectorStore│   │   │
│  │  └─────────┘ └────────┘ └────────────┘   │   │
│  └──────────────────────────────────────────┘   │
└─────────────────────┬───────────────────────────┘
           ┌──────────┴──────────┐
     ┌─────▼─────┐        ┌──────▼──────┐
     │  Ollama   │        │   Qdrant    │
     │  (local)  │        │  (vectors)  │
     └───────────┘        └─────────────┘
```

---

# Demo: Configuration Switching

## Providers

```json
{
  "AI": {
    "Provider": "Ollama",  // or "OpenAI"
    "Ollama": {
      "Endpoint": "http://localhost:11434",
      "ChatModel": "llama3.2",
      "EmbeddingModel": "nomic-embed-text"
    }
  }
}
```

---

# Demo: Document Ingestion

## Upload → Chunk → Embed → Store

```csharp
// 1. Chunk the document
var chunks = _chunker.Chunk(document);

// 2. Generate embeddings
var embeddings = await _embedder.EmbedAsync(chunks);

// 3. Store in vector database
await _vectorStore.UpsertAsync(chunks);
```

---

# Demo: Query Pipeline

## Visualizing the Process

1. **Query**: "What's the difference between espresso and pour-over?"
2. **Embed**: Convert to vector
3. **Search**: Find top 5 similar chunks
4. **Score**: Display relevance scores
5. **Augment**: Build context
6. **Generate**: Stream response

---

# Best Practices

## ✅ Do

- Test different chunk sizes
- Monitor relevance scores
- Use appropriate embedding models
- Implement caching
- Log everything for debugging

## ❌ Don't

- Mix embedding models
- Ignore context window limits
- Skip relevance filtering
- Forget about rate limits

---

# Resources

## Documentation
- [Microsoft.Extensions.AI](https://github.com/dotnet/extensions)
- [Qdrant Documentation](https://qdrant.tech/documentation/)
- [Ollama Models](https://ollama.ai/library)

## Papers
- "Retrieval-Augmented Generation for Knowledge-Intensive NLP Tasks" (Lewis et al., 2020)

## Tools
- This demo: `dojo.rag` repository

---

# Questions?

## Let's Discuss!

- RAG implementation details
- Vector database choices
- Embedding model selection
- Production considerations

---

# 🎯 Hands-On Exercise

1. Start the services:
   ```bash
   docker-compose up -d
   dotnet run --project src/Dojo.Rag.Api
   cd src/Dojo.Rag.UI && npm run dev
   ```

2. Upload sample documents

3. Ask questions and observe:
   - Retrieved chunks
   - Relevance scores
   - Token usage

4. Try switching embedding models!
