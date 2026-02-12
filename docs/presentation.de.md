---
marp: true
theme: default
paginate: true
backgroundColor: #1e1e2e
color: #cdd6f4
style: |
  @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&family=JetBrains+Mono:wght@400;500&display=swap');

  /* ===== BASE STYLES ===== */
  section {
    font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
    font-size: 24px;
    line-height: 1.6;
    padding: 40px 50px;
    background: linear-gradient(135deg, #1e1e2e 0%, #252535 50%, #1e1e2e 100%);
  }

  /* ===== TYPOGRAPHY ===== */
  h1 {
    font-size: 2.8em;
    font-weight: 700;
    color: #89b4fa;
    margin: 0 0 0.5em 0;
    letter-spacing: -0.02em;
    line-height: 1.2;
    background: linear-gradient(135deg, #89b4fa 0%, #b4befe 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }

  h2 {
    font-size: 2em;
    font-weight: 600;
    color: #cba6f7;
    margin: 0.8em 0 0.4em 0;
    letter-spacing: -0.01em;
    border-bottom: 2px solid #313244;
    padding-bottom: 0.3em;
  }

  h3 {
    font-size: 1.5em;
    font-weight: 600;
    color: #f5c2e7;
    margin: 0.6em 0 0.3em 0;
  }

  p {
    margin: 0.6em 0;
    color: #cdd6f4;
  }

  /* ===== LISTS ===== */
  ul, ol {
    margin: 0.8em 0;
    padding-left: 1.5em;
  }

  li {
    margin: 0.4em 0;
    color: #cdd6f4;
  }

  li::marker {
    color: #89b4fa;
  }

  ul li {
    position: relative;
  }

  /* ===== CODE BLOCKS ===== */
  code {
    font-family: 'JetBrains Mono', 'Fira Code', monospace;
    font-size: 0.85em;
    background: linear-gradient(135deg, #313244 0%, #3a3a52 100%);
    color: #a6e3a1;
    padding: 0.2em 0.4em;
    border-radius: 4px;
    border: 1px solid #45475a;
  }

  pre {
    background: linear-gradient(135deg, #181825 0%, #1e1e2e 100%);
    border: 1px solid #313244;
    border-radius: 8px;
    padding: 1em;
    margin: 1em 0;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
  }

  pre code {
    background: transparent;
    border: none;
    padding: 0;
    font-size: 0.8em;
    line-height: 1.5;
  }

  /* ===== LINKS ===== */
  a {
    color: #f5c2e7;
    text-decoration: none;
    border-bottom: 1px solid transparent;
    transition: border-color 0.2s ease;
  }

  a:hover {
    border-bottom-color: #f5c2e7;
  }

  /* ===== BLOCKQUOTES ===== */
  blockquote {
    border-left: 4px solid #89b4fa;
    padding: 0.8em 1.2em;
    margin: 1em 0;
    background: linear-gradient(90deg, rgba(137, 180, 250, 0.1) 0%, transparent 100%);
    border-radius: 0 8px 8px 0;
    color: #bac2de;
    font-style: italic;
  }

  blockquote p {
    margin: 0;
    color: #a6adc8;
  }

  /* ===== TABLES ===== */
  table {
    width: 100%;
    border-collapse: collapse;
    margin: 1em 0;
    font-size: 0.85em;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
  }

  th {
    background: linear-gradient(135deg, #313244 0%, #45475a 100%);
    color: #cdd6f4;
    font-weight: 600;
    padding: 0.8em 1em;
    text-align: left;
    border-bottom: 2px solid #585b70;
  }

  td {
    padding: 0.7em 1em;
    border-bottom: 1px solid #313244;
    color: #cdd6f4;
  }

  tr:nth-child(even) {
    background-color: rgba(49, 50, 68, 0.3);
  }

  tr:hover {
    background-color: rgba(137, 180, 250, 0.1);
  }

  /* ===== COLUMNS ===== */
  .columns {
    display: flex;
    gap: 2.5em;
    margin: 1em 0;
  }

  .col {
    flex: 1;
  }

  /* ===== SPECIAL SLIDE CLASSES ===== */
  /* Title slide styling */
  section.title {
    text-align: center;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
  }

  section.title h1 {
    font-size: 3.5em;
    margin-bottom: 0.3em;
  }

  section.title h2 {
    font-size: 1.5em;
    border: none;
    color: #a6adc8;
    font-weight: 400;
  }

  /* Section header slides */
  section.section {
    text-align: center;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }

  section.section h1 {
    font-size: 4em;
    margin: 0;
  }

  section.section h2 {
    border: none;
    color: #89b4fa;
  }

  /* Accent elements */
  .accent {
    color: #fab387;
    font-weight: 600;
  }

  .highlight {
    background: linear-gradient(90deg, rgba(250, 179, 135, 0.3) 0%, rgba(250, 179, 135, 0.1) 100%);
    padding: 0.1em 0.3em;
    border-radius: 4px;
  }

  /* Code/tech styling */
  .tech {
    font-family: 'JetBrains Mono', monospace;
    color: #94e2d5;
  }

  /* Info boxes */
  .info-box {
    background: linear-gradient(135deg, rgba(137, 180, 250, 0.15) 0%, rgba(180, 190, 254, 0.05) 100%);
    border: 1px solid rgba(137, 180, 250, 0.3);
    border-radius: 12px;
    padding: 1.2em;
    margin: 1em 0;
  }

  .warning-box {
    background: linear-gradient(135deg, rgba(250, 179, 135, 0.15) 0%, rgba(249, 226, 175, 0.05) 100%);
    border: 1px solid rgba(250, 179, 135, 0.3);
    border-radius: 12px;
    padding: 1.2em;
    margin: 1em 0;
  }

  .success-box {
    background: linear-gradient(135deg, rgba(166, 227, 161, 0.15) 0%, rgba(148, 226, 213, 0.05) 100%);
    border: 1px solid rgba(166, 227, 161, 0.3);
    border-radius: 12px;
    padding: 1.2em;
    margin: 1em 0;
  }

  /* ===== UTILITY CLASSES ===== */
  .text-center {
    text-align: center;
  }

  .text-sm {
    font-size: 0.85em;
  }

  .text-lg {
    font-size: 1.2em;
  }

  .mt-0 { margin-top: 0; }
  .mb-0 { margin-bottom: 0; }
  .mt-1 { margin-top: 1em; }
  .mb-1 { margin-bottom: 1em; }

  /* ===== PAGINATION ===== */
  section::after {
    content: attr(data-marpit-pagination) ' / ' attr(data-marpit-pagination-total);
    position: absolute;
    bottom: 20px;
    right: 30px;
    font-size: 0.7em;
    color: #6c7086;
    font-family: 'JetBrains Mono', monospace;
  }

  /* Hide pagination on title/section slides */
  section.title::after,
  section.section::after {
    display: none;
  }
---

# Retrieval-Augmented Generation (RAG)

### Intelligente Suche mit Vektordatenbanken aufbauen

![bg right:40% 80%](https://upload.wikimedia.org/wikipedia/commons/thumb/4/4b/Open_AI_Logo.svg/512px-Open_AI_Logo.svg.png)

---

# Was ist RAG?

**Retrieval-Augmented Generation** kombiniert:

1. **Information Retrieval** - Relevante Dokumente finden
2. **Language Models** - Menschenähnliche Antworten generieren

> "Gib dem LLM den Kontext, den es braucht, anstatt sich darauf zu verlassen, was es während des Trainings gelernt hat"

---

# Warum RAG?

### Probleme mit reinen LLMs

- 🧠 **Stichtag** - Trainingsdaten haben ein Zeitlimit
- 🎭 **Halluzinationen** - Selbstbewusst falsche Antworten
- 📦 **Keine privaten Daten** - Kein Zugriff auf Ihre Dokumente
- 💰 **Fine-Tuning ist teuer** - Erfordert erhebliche Ressourcen

---

# Warum RAG?

### RAG löst diese Probleme

- ✅ Aktuelles Wissen
- ✅ Basiert auf tatsächlichen Dokumenten
- ✅ Funktioniert mit privaten Daten
- ✅ Kein Modell-Retraining erforderlich

---

# RAG-Architektur-Überblick

```
┌─────────────┐    ┌─────────────┐     ┌─────────────┐
│   Nutzer    │    │   Vektor    │     │   LLM       │
│   Anfrage   │──▶│   Suche     │───▶│   Generier. │
└─────────────┘    └─────────────┘     └─────────────┘
                          │
                          ▼
                   ┌─────────────┐
                   │  Abgerufene │
                   │  Dokumente  │
                   └─────────────┘
```

---

# Die RAG-Pipeline

### Schritt-für-Schritt-Ablauf

1. **Ingest** - Dokumente laden und chunken
2. **Embed** - Chunks in Vektoren umwandeln
3. **Store** - In Vektordatenbank speichern
4. **Query** - Nutzer stellt eine Frage
5. **Retrieve** - Ähnliche Chunks finden
6. **Augment** - Prompt mit Kontext aufbauen
7. **Generate** - LLM erzeugt Antwort

---

# Schritt 1: Dokumenten-Chunking

### Warum Chunking?

- LLMs haben **Kontextfenster-Limits** (4K - 128K Token)
- Kleinere Chunks = **präziseres Retrieval**
- Größere Chunks = **mehr Kontext erhalten**

---

# Schritt 1: Dokumenten-Chunking

### Chunking-Strategien

| Strategie     | Beschreibung                 | Anwendungsfall   |
|---------------|------------------------------|------------------|
| Feste Größe   | Nach Zeichenanzahl teilen    | Einfach, schnell |
| Satz          | An Satzgrenzen teilen        | Erhält Bedeutung |
| Semantisch    | Nach Thema/Bedeutung teilen  | Beste Qualität   |
| Rekursiv      | Mehrere Strategien probieren | Ausgewogen       |

---

# Chunking: Die Überlappungsstrategie

```
Dokument: "The quick brown fox jumps over the lazy dog."

Chunk-Größe: 20 Zeichen | Überlappung: 5 Zeichen

Chunk 1: "The quick brown fox "
Chunk 2: " fox jumps over the "
Chunk 3: " the lazy dog."
         ↑↑↑↑↑
        Überlappung erhält Kontext
```

> **Faustregel**: 10-20% Überlappung verhindert Kontextverlust an Chunk-Grenzen

---

# Schritt 2: Embeddings

### Was sind Embeddings?

Embeddings sind **Vektorrepräsentationen** von Text, die semantische Bedeutung enthalten.

```
"coffee brewing"  → [0.12, -0.45, 0.78, 0.23, ...]
"making espresso" → [0.14, -0.42, 0.81, 0.19, ...]  ← Ähnlich!
"car engine"      → [-0.67, 0.32, -0.15, 0.89, ...] ← Unterschiedlich!
```

### Kernelement

Ähnliche Bedeutungen → Ähnliche Vektoren → Kleine Distanz

---

# Embedding-Modelle

| Modell                 | Dimensionen | Anbieter    |
|------------------------|-------------|-------------|
| text-embedding-3-small | 1536        | OpenAI      |
| text-embedding-3-large | 3072        | OpenAI      |
| nomic-embed-text       | 768         | Ollama      |
| all-MiniLM-L6-v2       | 384         | HuggingFace |

---

# Embedding-Modelle

### Überlegungen

- **Dimensionen** beeinflussen Speicher und Suchgeschwindigkeit
- **Qualität** variiert je nach Anwendungsfall
- **Lokal vs. API** - Abwägungen

---

# Schritt 3: Vektordatenbanken

### Warum keine regulären Datenbanken?

```sql
-- Das funktioniert nicht für semantische Suche!
SELECT * FROM documents
WHERE content LIKE '%coffee brewing%'
```
---

# Schritt 3: Vektordatenbanken

### Vektor-DB-Features

- 🔍 **Ähnlichkeitssuche** - Nächste Nachbarn finden
- 📊 **ANN-Algorithmen** - Approximate Nearest Neighbor
- 🏎️ **Optimierte Indizes** - HNSW, IVF, etc.
- 📈 **Skalierbarkeit** - Milliarden von Vektoren

---

# Vektordatenbank-Optionen

### Beliebte Optionen

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

---

# Vektor-Ähnlichkeitsmetriken

### Distanzfunktionen

| Metrik        | Formel     | Anwendungsfall              |
|---------------|------------|-----------------------------|
| Cosinus       | 1 - cos(θ) | Am häufigsten, normalisiert |
| Euklidisch    | √Σ(a-b)²   | Absolute Distanzen          |
| Skalarprodukt | Σ(a×b)     | Größenordnung wichtig       |

### Cosinus-Ähnlichkeit

```
sim(A, B) = (A · B) / (||A|| × ||B||)

Bereich: -1 bis 1 (1 = identisch)
```

---

# Schritt 4: Der Query-Prozess

```
Nutzer: "How do I make espresso?"
          │
          ▼
┌──────────────────────────────────────┐
│  1. Query embedden                   │
│     → [0.12, -0.45, 0.78, ...]       │
├──────────────────────────────────────┤
│  2. Vektordatenbank durchsuchen      │
│     → Top K ähnliche Chunks          │
├──────────────────────────────────────┤
│  3. Nach Relevanz-Score ranken       │
│     → Unterhalb Schwelle filtern     │
└──────────────────────────────────────┘
```

---

# Schritt 5: Kontext-Erweiterung

### Den Prompt aufbauen

```
System: Du bist ein hilfreicher Assistent. Verwende den folgenden
Kontext, um Fragen zu beantworten. Wenn der Kontext die
Antwort nicht enthält, sage "Ich weiß es nicht."

Kontext:
---
[Abgerufener Chunk 1: Espresso-Grundlagen...]
[Abgerufener Chunk 2: Druck und Temperatur...]
[Abgerufener Chunk 3: Wichtigkeit der Mahlgröße...]
---

Nutzer: How do I make espresso?
```

---

# Die RAG-Prompt-Vorlage

```csharp
var prompt = $"""
Du bist ein hilfreicher Assistent, der Fragen basierend auf
dem bereitgestellten Kontext beantwortet.

KONTEXT:
{string.Join("\n---\n", retrievedChunks)}

REGELN:
- Verwende nur Informationen aus dem obigen Kontext
- Falls unsicher, sage "Ich habe nicht genügend Informationen"
- Zitiere Quellen wenn möglich

NUTZERFRAGE: {userQuery}
""";
```

---

# Schritt 6: Generierung

### Das LLM macht seine Magie

- Liest den **erweiterten Prompt**
- Generiert eine **fundierte Antwort**
- Kann **Zitate** zu Quelldokumenten enthalten

### Beispielantwort

> "Um Espresso zuzubereiten, müssen Sie 9 Bar Druck
> und Wasser bei 90-96°C verwenden. Der Mahlgrad sollte extra-fein sein,
> und die Extraktion dauert 25-30 Sekunden für einen ordentlichen Shot."
> 
> *Quellen: espresso-basics.md*

---

# Wichtige RAG-Parameter

### Feinabstimmung für Qualität

| Parameter       | Beschreibung                   | Typ. Wert |
|-----------------|--------------------------------|-----------|
| **Chunk-Größe** | Zeichen pro Chunk              | 500-1000  |
| **Überlappung** | Gemeinsame Zeichen zw. Chunks  | 50-200    |
| **Top K**       | Anzahl der abzurufenden Chunks | 3-10      |
| **Min. Score**  | Relevanz-Schwelle              | 0.7-0.8   |

---

# Häufige RAG-Herausforderungen

### 1. Retrieval-Qualität

- Falsche Chunks abgerufen
- **Lösung**: Besseres Chunking, Hybrid-Suche

### 2. Lost in the Middle

- LLMs ignorieren mittleren Kontext
- **Lösung**: Reranking, Chunks limitieren

### 3. Kontextfenster-Limits

- Zu viele Chunks = Abschneiden
- **Lösung**: Zusammenfassung, Auswahl

---

# Fortgeschrittene Techniken

### Hybrid Search
Kombiniert Vektor-Ähnlichkeit + Keyword-Matching (BM25)

### Query Expansion
Schreibt Queries für besseres Retrieval um

### Hypothetical Document Embeddings (HyDE)
Generiert hypothetische Antwort, embeddet diese

### Reranking
Verwendet Cross-Encoder zum Neuordnen der Ergebnisse

---

# Multi-Model-Überlegungen

### Embedding-Modell-Collections

```
documents_nomic-embed-text     ← Ollama-Embeddings
documents_text-embedding-3-small ← OpenAI-Embeddings
```

> ⚠️ Unterschiedliche Modelle = Unterschiedliche Vektorräume
> 
> **Niemals Embeddings von verschiedenen Modellen mischen!**

---

# Demo-Architektur

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
            ┌─────────┴──────────┐
      ┌─────▼─────┐       ┌──────▼──────┐
      │  Ollama   │       │   Qdrant    │
      │  (lokal)  │       │  (Vektoren) │
      └───────────┘       └─────────────┘
```

---

# Demo: Konfigurationswechsel

### Anbieter

```json
{
  "AI": {
    "Provider": "Ollama",  // oder "OpenAI"
    "Ollama": {
      "Endpoint": "http://localhost:11434",
      "ChatModel": "llama3.2",
      "EmbeddingModel": "nomic-embed-text"
    }
  }
}
```

---

# Demo: Dokumenten-Ingestion

### Upload → Chunk → Embed → Store

```csharp
// 1. Dokument chunken
var chunks = _chunker.Chunk(document);

// 2. Embeddings generieren
var embeddings = await _embedder.EmbedAsync(chunks);

// 3. In Vektordatenbank speichern
await _vectorStore.UpsertAsync(chunks);
```

---

# Demo: Query-Pipeline

1. **Query**: "What's the difference between espresso and pour-over?"
2. **Embed**: In Vektor umwandeln
3. **Search**: Top 5 ähnliche Chunks finden
4. **Score**: Relevanz-Scores anzeigen
5. **Augment**: Kontext aufbauen
6. **Generate**: Antwort streamen

---

# Best Practices

<div class="columns">
<div class="col">
✅ Gut

- Verschiedene Chunk-Größen testen
- Relevanz-Scores überwachen
- Passende Embedding-Modelle verwenden
- Caching implementieren
- Alles für Debugging loggen
</div>
<div class="col">
❌ Schlecht

- Embedding-Modelle mischen
- Kontextfenster-Limits ignorieren
- Relevanzfilterung überspringen
- Rate-Limits vergessen
</div>
</div>

---

# Ressourcen

### Dokumentation
- [Microsoft.Extensions.AI: https://github.com/dotnet/extensions](https://github.com/dotnet/extensions)
- [Qdrant Documentation: https://qdrant.tech/documentation/](https://qdrant.tech/documentation/)
- [Ollama Models: https://ollama.ai/library](https://ollama.ai/library)
- u.v.m.

### Demo Repository
- https://github.com/AliMoinzadeh/Dojo.RAG

---

# Fragen?
