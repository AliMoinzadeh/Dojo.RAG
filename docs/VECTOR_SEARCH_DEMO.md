# Vector Search Demo - Dokumentation

## Übersicht

Die Vector Search Demo ist eine interaktive Seite, die die Limitationen der reinen Vektorsuche demonstriert und verschiedene Techniken zeigt, um die Suchqualität zu verbessern.

## Aktuelle Features

### 1. Standard Vektorsuche
- Reine semantische Suche basierend auf Cosine-Ähnlichkeit
- Zeigt typische Limitationen wie Synonym-Probleme, Negationen, Fachbegriffe

### 2. Hybrid Search
**Status:** ✅ Implementiert

Kombiniert Vektor-Ähnlichkeit mit Keyword-Matching (BM25-ähnlich).

**Vorteile:**
- Findet exakte Begriffe auch bei niedriger semantischer Ähnlichkeit
- Nutzt Tags/Kategorien für besseres Matching
- Partial Matching für Wortteile

**Implementierung:**
- [HybridSearchService.cs](../src/Dojo.Rag.Api/Services/HybridSearchService.cs)
- Gewichtung: 70% Vektor, 30% Keyword (konfigurierbar)

### 3. Query Expansion
**Status:** ✅ Implementiert

Verwendet ein LLM um die Suchanfrage mit Synonymen und verwandten Begriffen zu erweitern.

**Vorteile:**
- Findet Dokumente mit unterschiedlicher Terminologie
- Überbrückt Synonym-Lücken
- Besonders effektiv für Fachbegriffe

**Implementierung:**
- [QueryExpansionService.cs](../src/Dojo.Rag.Api/Services/QueryExpansionService.cs)
- Nutzt Chat-Model für Expansion

---

## Geplante Verbesserungen

### 4. Reranking (Cross-Encoder)
**Status:** ✅ Implementiert

**Beschreibung:**
Nach der initialen Vektorsuche werden die Top-K Ergebnisse mit einem Cross-Encoder Model neu bewertet. Cross-Encoder betrachten Query und Dokument gemeinsam und können feinere Relevanz-Urteile treffen.

**Vorteile:**
- Höhere Präzision als Bi-Encoder (Embedding-basiert)
- Kann Kontext besser verstehen
- Besonders gut bei ambigen Queries

**Implementierung:**
- [RerankingService.cs](../src/Dojo.Rag.Api/Services/RerankingService.cs)
- Demo-Flow: `PerformEnhancedSearchAsync` mit optionalem Reranking

**Hinweis:**
- Aktuell LLM-basiertes Reranking (für Demo-Zwecke) mit Fallback auf Original-Reihenfolge

**Aufwand:** Mittel - benötigt zusätzliches ML-Model oder API-Integration

---

### 5. Semantic Chunking
**Status:** ✅ Implementiert

**Beschreibung:**
Statt Dokumente nach fixer Zeichenzahl zu chunken, werden semantische Grenzen verwendet (Absätze, Themenübergänge).

**Vorteile:**
- Chunks enthalten vollständige Gedanken
- Bessere Embedding-Qualität
- Weniger abgeschnittene Sätze

**Implementierung:**
- [DocumentChunker.cs](../src/Dojo.Rag.Api/Services/DocumentChunker.cs)
- Aktivierung per `Rag:UseSemanticChunking` in [appsettings.json](../src/Dojo.Rag.Api/appsettings.json)

**Ansatz:**
- Satzgrenzen-basiertes Aggregieren auf Ziel-Chunk-Groesse

**Aufwand:** Mittel - Anpassung in DocumentChunker

---

### 6. Min-Score Threshold Slider
**Status:** ✅ Implementiert

**Beschreibung:**
Dynamische Anpassung des Minimum-Relevanz-Scores zur Laufzeit über einen UI-Slider.

**Vorteile:**
- Benutzer kann Precision/Recall Trade-off steuern
- Gut für Demos und Experimente
- Einfach zu verstehen

**Implementierung:**
- Frontend: Range-Slider (0.0 - 1.0)
- Backend: `minScore` Parameter im Search-Endpoint
- Filterung in Standard und Enhanced Search

**Aufwand:** Gering - nur UI + Parameter

---

### 7. Multi-Vector Search
**Status:** ✅ Implementiert (Demo)

**Beschreibung:**
Mehrere Embeddings pro Chunk speichern: Titel/Überschrift, Hauptinhalt, extrahierte Keywords separat embedden.

**Vorteile:**
- Spezifischere Suche möglich
- Gewichtung verschiedener Aspekte
- Bessere Ergebnisse bei strukturierten Dokumenten

**Implementierung:**
- [MultiVectorSearchService.cs](../src/Dojo.Rag.Api/Services/MultiVectorSearchService.cs)
- Separate Embeddings fuer Inhalt und Tags, gewichtete Kombination

**Hinweis:**
- Aktuell demo-spezifisch (Inhalt + Tags), kein Schema-Change in Qdrant

---

### 8. Contextual Embeddings
**Status:** ✅ Implementiert

**Beschreibung:**
Beim Embedding eines Chunks wird der Kontext (vorheriger/nächster Absatz, Dokumenttitel) mit einbezogen.

**Vorteile:**
- Chunks haben mehr Kontext-Information im Embedding
- Bessere Disambiguation
- Hilfreich bei kurzen Chunks

**Implementierung:**
- Demo-Flow: Kontext-Embedding im Demo-Index (Kategorie + Tags + Text)
- Aktivierung per Toggle `UseContextualEmbeddings`

**Hinweis:**
- Kontext wird im Demo als "Kategorie/Tags/Text" zusammengefuehrt

---

### 9. HyDE (Hypothetical Document Embeddings)
**Status:** ✅ Implementiert

**Beschreibung:**
Statt die Query direkt zu embedden, generiert ein LLM zuerst ein hypothetisches Dokument, das die Antwort enthalten würde. Dieses wird dann embedded.

**Vorteile:**
- Query und Dokument sind im gleichen "Stil"
- Besser bei kurzen/vagen Queries
- Kann fehlende Begriffe ergänzen

**Implementierung:**
- [HyDEService.cs](../src/Dojo.Rag.Api/Services/HyDEService.cs)
- Demo-Flow: HyDE-Text wird embedded statt der Query

**Hinweis:**
- Im UI wird der erzeugte HyDE-Text angezeigt

---

### 10. Graph-Vector Search (HNSW)
**Status:** ✅ Implementiert (Demo-Simulation)

**Beschreibung:**
Verwendet einen graphbasierten ANN-Index (HNSW = Hierarchical Navigable Small World), um ähnliche Vektoren schneller zu finden. Die Suche ist approximativ und lässt sich ueber Parameter zwischen Recall und Geschwindigkeit steuern.

**Vorteile:**
- Deutlich schnellere Suche bei grossen Datenmengen
- Skalierbar fuer Echtzeit-Use-Cases
- Recall/Speed-Tradeoff konfigurierbar (z. B. über `efSearch`)

**Implementierung:**
- Demo-Flow: Approximate Candidate-Pass mit `efSearch`-Slider
- Aktivierung per Toggle `UseHnswApproximate`

**Hinweis:**
- In der Demo wird HNSW als Kandidaten-Auswahl simuliert

---

## Demo-Szenarien

Die JSON-Datei `docs/demo-sentences.json` enthält vordefinierte Szenarien, die spezifische Limitationen demonstrieren:

| Szenario | Query | Problem | Beste Lösung |
|----------|-------|---------|--------------|
| Synonym-Problem | "Java Getränk" | Java ≠ Kaffee semantisch | Query Expansion |
| Negations-Problem | "Kaffee ohne Hitze" | Negation schwer zu verstehen | Hybrid Search |
| Fachbegriff-Problem | "Schaum auf dem Kaffee" | Crema nicht erkannt | Query Expansion |
| Zahlen-Problem | "Temperatur unter 95 Grad" | Numerischer Vergleich | Hybrid Search |

---

## API-Endpunkte

### GET /api/vectorsearchdemo/sentences
Gibt alle Demo-Sätze und -Szenarien zurück.

### POST /api/vectorsearchdemo/initialize
Generiert Embeddings für alle Demo-Sätze. Muss vor der Suche aufgerufen werden.

### POST /api/vectorsearchdemo/search
Führt Suche mit optionalen Verbesserungen durch.

**Request:**
```json
{
  "query": "Kaffee Temperatur",
  "enhancements": {
    "useHybridSearch": true,
    "useQueryExpansion": false,
    "useReranking": true,
    "useMultiVectorSearch": true,
    "useContextualEmbeddings": true,
    "useHyDE": true,
    "useHnswApproximate": true,
    "hnswEfSearch": 32
  },
  "topK": 5,
  "minScore": 0.5
}
```

**Response:**
```json
{
  "standardResults": { ... },
  "enhancedResults": { ... },
  "originalQuery": "Kaffee Temperatur",
  "appliedEnhancements": { ... }
}
```

**Hinweis (Response):**
- `enhancedResults.expandedQuery` zeigt die Query Expansion
- `enhancedResults.hypotheticalDocument` zeigt den HyDE-Text

### GET /api/vectorsearchdemo/status
Gibt den aktuellen Status der Demo zurück (initialisiert, Anzahl Embeddings).

---

## Architektur

```
┌─────────────────────────────────────────────────────────────┐
│                    VectorSearchDemo.tsx                      │
├─────────────────┬───────────────────┬───────────────────────┤
│ SentenceList    │ SearchInput       │ EnhancementToggles    │
│ (Demo-Sätze)    │ (Query + MinScore)│ (Hybrid/Expansion/...)│
├─────────────────┴───────────────────┴───────────────────────┤
│                 Side-by-Side Results                         │
│  ┌──────────────────────┬──────────────────────┐            │
│  │ Standard Suche       │ Mit Verbesserungen   │            │
│  │ - Ergebnis 1 (72%)   │ - Ergebnis 1 (89%)   │            │
│  │ - Ergebnis 2 (65%)   │ - Ergebnis 2 (85%)   │            │
│  └──────────────────────┴──────────────────────┘            │
└─────────────────────────────────────────────────────────────┘
            │
            ▼
        ┌───────────────────────────────┐
        │ VectorSearchDemoController    │
        └───────────────────────────────┘
            │
    ┌───────────────────┼───────────────────────┐
    ▼                   ▼                       ▼
┌─────────────────┐ ┌─────────────────┐     ┌─────────────────────┐
│ EmbeddingService│ │HybridSearchSvc  │     │MultiVectorSearchSvc │
└─────────────────┘ └─────────────────┘     └─────────────────────┘
    │                   │                       │
    ▼                   ▼                       ▼
  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
  │QueryExpansionSvc │  │MinScore Filter   │  │MinScore Filter   │
  └──────────────────┘  └──────────────────┘  └──────────────────┘
            │
            ▼
          ┌─────────────────┐
          │ RerankingSvc    │
          └─────────────────┘
```

---

## Weiterentwicklung

Um ein neues Enhancement hinzuzufügen:

1. **Service erstellen:** `src/Dojo.Rag.Api/Services/New<Feature>Service.cs`
2. **DI registrieren:** In `Program.cs` hinzufügen
3. **Models erweitern:** `SearchEnhancements` um neues Flag erweitern
4. **Controller anpassen:** Logik in `PerformEnhancedSearchAsync` einbauen
5. **Frontend Toggle:** In `VectorSearchDemo.tsx` neuen Toggle hinzufügen
6. **Types erweitern:** TypeScript-Interface aktualisieren
