---
marp: true
theme: default
paginate: true
backgroundColor: #1e1e2e
color: #cdd6f4
style: |
  @import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&family=JetBrains+Mono:wght@400;500&display=swap');

  /* ===== BASE ===== */
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

  p { margin: 0.6em 0; color: #cdd6f4; }

  /* ===== LISTS ===== */
  ul, ol { margin: 0.8em 0; padding-left: 1.5em; }
  li { margin: 0.4em 0; color: #cdd6f4; }
  li::marker { color: #89b4fa; }

  /* ===== CODE ===== */
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
    font-size: 0.9em;
    line-height: 1.5;
    color: #cdd6f4;
  }

  .hljs { color: #cdd6f4; background: transparent; }
  .hljs-comment, .hljs-quote { color: #6c7086; font-style: italic; }
  .hljs-keyword, .hljs-selector-tag, .hljs-built_in, .hljs-doctag, .hljs-meta { color: #cba6f7; font-weight: 600; }
  .hljs-string, .hljs-regexp, .hljs-symbol, .hljs-bullet { color: #a6e3a1; }
  .hljs-title, .hljs-section, .hljs-function, .hljs-attr { color: #74c7ec; font-weight: 600; }
  .hljs-type, .hljs-class, .hljs-name { color: #89b4fa; }
  .hljs-number, .hljs-literal { color: #fab387; }
  .hljs-variable, .hljs-template-variable, .hljs-params { color: #f5c2e7; }
  .hljs-subst { color: #cdd6f4; }

  /* ===== LINKS / QUOTES / TABLES ===== */
  a { color: #f5c2e7; text-decoration: none; border-bottom: 1px solid transparent; }
  a:hover { border-bottom-color: #f5c2e7; }

  blockquote {
    border-left: 4px solid #89b4fa;
    padding: 0.8em 1.2em;
    margin: 1em 0;
    background: linear-gradient(90deg, rgba(137, 180, 250, 0.1) 0%, transparent 100%);
    border-radius: 0 8px 8px 0;
    color: #bac2de;
    font-style: italic;
  }
  blockquote p { margin: 0; color: #a6adc8; }

  /* Tabellen: helle Karte auf dunklem Slide für besseren Kontrast.
     fit-content + auto-margin zentriert die Tabelle und schrumpft sie
     auf Inhaltsbreite, damit kein Leerraum rechts entsteht.
     section-Selektor schlägt die width:100%-Regel des Default-Themes. */
  section table {
    width: fit-content;
    max-width: 100%;
    border-collapse: collapse;
    margin: 1em auto;
    font-size: 0.85em;
    border-radius: 10px;
    overflow: hidden;
    box-shadow: 0 4px 15px rgba(0, 0, 0, 0.35);
    background: #eff1f5;
  }
  th {
    background: linear-gradient(135deg, #ccd0da 0%, #bcc0cc 100%);
    color: #1e1e2e;
    font-weight: 700;
    padding: 0.8em 1em;
    text-align: left;
    border-bottom: 2px solid #9ca0b0;
  }
  td {
    padding: 0.7em 1em;
    border-bottom: 1px solid #dce0e8;
    color: #1e1e2e;
    background: #eff1f5;
  }
  tr:nth-child(even) td { background-color: #e6e9ef; }
  td strong, td b { color: #11111b; }
  td code { background: #dce0e8; color: #5c2c8b; border-color: #bcc0cc; }

  /* ===== COLUMNS ===== */
  .columns { display: flex; gap: 2.5em; margin: 1em 0; }
  .col { flex: 1; }

  /* ===== SPECIAL SLIDES ===== */
  section.title {
    text-align: center;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
  }
  section.title h1 { font-size: 3.5em; margin-bottom: 0.3em; }
  section.title h2 { font-size: 1.5em; border: none; color: #a6adc8; font-weight: 400; }

  section.section {
    text-align: center;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }
  section.section h1 { font-size: 4em; margin: 0; }
  section.section h2 { border: none; color: #89b4fa; }
  section.section .kicker {
    font-family: 'JetBrains Mono', monospace;
    color: #6c7086;
    letter-spacing: 0.3em;
    text-transform: uppercase;
    font-size: 0.7em;
    margin-bottom: 1em;
  }

  /* ===== BOXES ===== */
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

  .accent { color: #fab387; font-weight: 600; }
  .tech { font-family: 'JetBrains Mono', monospace; color: #94e2d5; }

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
  section.title::after, section.section::after { display: none; }

  .mermaid-diagram { margin: 0.8em 0; text-align: center; }
  .mermaid-diagram img {
    max-width: 100%;
    max-height: 460px;
    border-radius: 10px;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.35);
  }
---

<!-- _class: title -->

# RAG & Vektorsuche

## Wie LLMs lernen, in *euren* Daten zu suchen

---

# Das Problem

Ein LLM allein kennt nur das, was es **beim Training gesehen hat**.

- 🧠 **Stichtag**: das Wissen veraltet
- 🎭 **Halluzinationen**: selbstbewusst falsche Antworten
- 📦 **Keine privaten Daten**: keine Firmen­dokumente, keine Tickets, keine Wiki-Seiten
- 💰 **Fine-Tuning** ist teuer und träge

> Wie geben wir dem Modell aktuellen, projekt­spezifischen Kontext
> ohne es neu zu trainieren?

---

# Die Idee von RAG

**Retrieval-Augmented Generation:**
Wir **suchen** die passenden Dokumente und **schicken sie mit der Frage** ans LLM.

```mermaid
%%{init: {'theme': 'dark', 'themeVariables': {'primaryColor': '#313244', 'primaryTextColor': '#cdd6f4', 'lineColor': '#6c7086', 'fontSize': '18px'}}}%%
flowchart LR
  U(["Frage"]) --> R["Retrieval<br/>(Vektorsuche)"]
  DB[("Dokumente")] -.-> R
  R -->|"Frage + Kontext"| L["LLM"]
  L --> A(["Fundierte Antwort"])

  classDef q fill:#89b4fa,color:#1e1e2e,stroke:none,font-weight:bold
  classDef r fill:#a6e3a1,color:#1e1e2e,stroke:none,font-weight:bold
  classDef db fill:#f9e2af,color:#1e1e2e,stroke:none,font-weight:bold
  classDef l fill:#cba6f7,color:#1e1e2e,stroke:none,font-weight:bold
  classDef a fill:#f38ba8,color:#1e1e2e,stroke:none,font-weight:bold

  class U q
  class R r
  class DB db
  class L l
  class A a
  linkStyle default stroke:#585b70,stroke-width:2px
```

Der Trick steckt im **Retrieval**, und genau dafür brauchen wir Vektorsuche.

---

# Roadmap für heute

<div class="columns">
<div class="col">

### Teil 1: Vektorsuche
das **Fundament**

- Warum Keyword-Suche nicht reicht
- Embeddings & Cosinus
- Vektordatenbanken
- HNSW: warum es schnell ist

</div>
<div class="col">

### Teil 2: RAG
die **Pipeline**

- Chunking & Ingestion
- Retrieve → Augment → Generate
- Stolpersteine
- Demo

</div>
</div>

---

<!-- _class: section -->

<div class="kicker">Teil 1</div>

# Vektorsuche

## Bedeutung statt Buchstaben

---

# Warum keine klassische Suche?

```sql
SELECT * FROM documents
WHERE content LIKE '%Kaffee brühen%';
```

Findet **nur** Dokumente, die diese **Wörter wortwörtlich** enthalten.

<div class="warning-box">

**Was klassische Suche nicht findet:**

- *„Java zubereiten"* (Synonym)
- *„Espresso machen"* (andere Terminologie)
- *„Brewing coffee"* (andere Sprache)
- *„Crema entsteht"* (Fachbegriff statt Beschreibung)

</div>

Wir wollen **Bedeutung** finden, nicht **Zeichenketten**.

---

# Embeddings: Bedeutung als Vektor

Ein **Embedding-Modell** wandelt Text in einen Zahlenvektor um.
Texte mit ähnlicher Bedeutung landen **nah beieinander** im Vektorraum.

```text
"Kaffee brühen"   → [ 0.12, -0.45,  0.78,  0.23, ... ]
"Espresso machen" → [ 0.14, -0.42,  0.81,  0.19, ... ]   ← ähnlich!
"Auto reparieren" → [-0.67,  0.32, -0.15,  0.89, ... ]   ← weit weg
```

<div class="info-box">

**Kernidee:**
Semantische Ähnlichkeit wird zu **geometrischer Nähe**.

</div>

Typische Dimensionen: **384 – 3072** pro Vektor.

---

# Cosinus-Ähnlichkeit

Wir messen nicht den **Abstand** der Vektoren, sondern den **Winkel** zwischen ihnen.

<div class="columns">
<div class="col">

```
sim(A, B) = (A · B) / (‖A‖ × ‖B‖)
```

| Wert     | Bedeutung                |
|----------|--------------------------|
| **1.0**  | Gleiche Richtung, identisch |
| **~0.7** | Verwandt                 |
| **0.0**  | Unabhängig               |
| **-1.0** | Gegenteil                |

</div>
<div class="col">

<div class="info-box">

Cosinus ist **Standard für Text-
Embeddings**, weil die *Länge*
des Vektors (= wie viel Text)
egal ist und nur die *Richtung*
(= Bedeutung) zählt.

</div>

</div>
</div>

---

# Distanzmetriken im Überblick

| Metrik           | Misst             | Wann verwenden?                          |
|------------------|-------------------|------------------------------------------|
| **Cosinus**      | Winkel            | Text-Embeddings (Standard)               |
| **Euklidisch**   | Geradlinige Distanz | Absolute Abstände (Clustering, GPS)    |
| **Skalarprodukt**| Winkel + Länge    | Wenn Magnitude wichtig ist (Ranking)     |

Bei **normalisierten Embeddings** (`‖v‖ = 1`) sind Cosinus und Skalarprodukt
mathematisch äquivalent. Die meisten modernen Modelle liefern bereits normalisierte Vektoren.

---

# Was ist eine Vektordatenbank?

Ein Speichersystem, das auf **Ähnlichkeitssuche in hochdimensionalen Vektorräumen** spezialisiert ist.

<div class="columns">
<div class="col">

### Was sie können muss

- 🗄️ **Vektoren speichern** + Metadaten (Payload)
- 🔍 **Top-K Ähnlichkeitssuche** in Millisekunden
- 🧭 **Filter auf Metadaten**
  (`tags = "espresso" AND lang = "de"`)
- 📈 **Skalieren** auf Millionen / Milliarden Vektoren
- 💾 **Persistieren** & Replizieren

</div>
<div class="col">

### Bekannte Vertreter

- **Qdrant** ⭐ (unsere Demo)
- Milvus
- Weaviate
- Pinecone (Cloud)
- ChromaDB
- **pgvector** (PostgreSQL-Extension,
  wenn man keine zweite DB will)

</div>
</div>

---

# Warum nicht einfach alle Vektoren vergleichen?

Bei **N Vektoren** und Dimension **d** kostet eine vollständige Suche
`O(N · d)`, also **linear in der Datenmenge**.

| Anzahl Vektoren | Cosinus-Vergleiche pro Query |
|-----------------|------------------------------|
| 10 000          | 10 000                       |
| 1 000 000       | 1 000 000                    |
| 1 000 000 000   | 1 000 000 000 😱             |

Bei Milliarden Vektoren brauchen wir einen **Index**, der die Suche
auf wenige tausend Vergleiche reduziert.

> Die Antwort heißt **ANN**: Approximate Nearest Neighbor.

---

# HNSW: der Standard-Index

**HNSW** = *Hierarchical Navigable Small World*, ein **graphbasierter ANN-Index**.

```mermaid
%%{init: {'theme': 'dark', 'themeVariables': {'primaryColor': '#313244', 'primaryTextColor': '#cdd6f4', 'lineColor': '#6c7086', 'fontSize': '16px'}}}%%
flowchart TB
  subgraph L2["Layer 2 &nbsp;&nbsp; wenige Knoten, weite Sprünge"]
    direction LR
    a2((&nbsp;)) --- b2((&nbsp;)) --- c2((&nbsp;)) --- d2((&nbsp;))
  end
  subgraph L1["Layer 1 &nbsp;&nbsp; mehr Knoten, mittlere Sprünge"]
    direction LR
    a1((&nbsp;)) --- b1((&nbsp;)) --- c1((&nbsp;)) --- d1((&nbsp;)) --- e1((&nbsp;)) --- f1((&nbsp;))
  end
  subgraph L0["Layer 0 &nbsp;&nbsp; alle Vektoren, kurze Schritte"]
    direction LR
    a0((&nbsp;)) --- b0((&nbsp;)) --- c0((&nbsp;)) --- d0((&nbsp;)) --- e0((&nbsp;)) --- f0((&nbsp;)) --- g0((&nbsp;)) --- h0((&nbsp;)) --- i0((&nbsp;))
  end

  L2 ==> L1 ==> L0

  classDef node fill:#89b4fa,color:#1e1e2e,stroke:none
  class a2,b2,c2,d2,a1,b1,c1,d1,e1,f1,a0,b0,c0,d0,e0,f0,g0,h0,i0 node
  linkStyle 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 stroke:#585b70,stroke-width:2px
  linkStyle 16,17 stroke:#89b4fa,stroke-width:4px
```

Die Suche **steigt von oben nach unten** ab: erst grobe Sprünge, dann Feinsuche.

---

# HNSW: wie die Suche läuft

<div class="columns">
<div class="col">

### Das Bild dahinter

Ein **Straßennetz in mehreren Ebenen:**

- **Oben (Layer 2)**, die Autobahn:
  wenige Knoten, weite Sprünge
- **Mitte (Layer 1)**, die Landstraße:
  mehr Knoten, mittlere Sprünge
- **Unten (Layer 0)**, alle Vektoren:
  kurze Schritte zum Ziel

</div>
<div class="col">

### Der Ablauf

Die Suche **startet oben**, springt grob
in die richtige Region und steigt
**Layer für Layer** ab.

<div class="info-box">

Statt **N Vergleiche** (linear)
nur etwa **log N** Vergleiche.

</div>

</div>
</div>

---

# HNSW in der Praxis

<div class="columns">
<div class="col">

### Stärken

- **„Small World"**: jeder Knoten erreicht
  jeden anderen in wenigen Schritten
- **Hierarchie** beschleunigt die Annäherung
- Bewährt bei Milliarden Vektoren in
  produktiven Systemen

</div>
<div class="col">

<div class="info-box">

**Wichtig fürs Verständnis:**

HNSW ist in **Qdrant immer aktiv**.
Wir können den Index nicht
ausschalten.

In der Demo verstellen wir nur den
Parameter `efSearch`.

</div>

</div>
</div>

---

# Der Drehregler *efSearch*

<div class="columns">
<div class="col">

`efSearch` bestimmt, wie viele Knoten die
Suche besucht, bevor sie aufhört:

- **hoch**: mehr Knoten besucht,
  findet die echten Nachbarn
  zuverlässiger, aber langsamer
- **niedrig**: weniger Knoten besucht,
  schneller, übersieht aber öfter
  gute Treffer

</div>
<div class="col">

<div class="info-box">

**Recall** sagt, wie vollständig die
Suche die *wirklich* ähnlichsten
Treffer findet.

Da HNSW approximativ arbeitet, ist
das nicht garantiert:

- **100 %**: alle echten Top-Treffer
  gefunden
- **80 %**: einer von fünf verpasst

</div>

</div>
</div>

---

<!-- _class: section -->

<div class="kicker">Teil 2</div>

# RAG

## Vom Dokument zur Antwort

---

# Die RAG-Pipeline

```mermaid
%%{init: {'theme': 'dark', 'themeVariables': {'primaryColor': '#313244', 'primaryTextColor': '#cdd6f4', 'lineColor': '#6c7086', 'fontSize': '16px'}}}%%
flowchart LR
  subgraph Ingest["1. Ingestion (einmalig pro Dokument)"]
    D["Dokument"] --> CH["Chunken"] --> EM1["Embedden"] --> ST[("Vektor-DB")]
  end

  subgraph Query["2. Query (pro Anfrage)"]
    Q(["Frage"]) --> EM2["Embedden"] --> SR["Suchen<br/>(Top-K)"] --> AU["Prompt bauen"] --> LLM["LLM"] --> AN(["Antwort"])
  end

  ST -.->|"Top-K Chunks"| SR

  classDef ing fill:#a6e3a1,color:#1e1e2e,stroke:none,font-weight:bold
  classDef qry fill:#89b4fa,color:#1e1e2e,stroke:none,font-weight:bold
  classDef store fill:#f9e2af,color:#1e1e2e,stroke:none,font-weight:bold
  classDef llm fill:#cba6f7,color:#1e1e2e,stroke:none,font-weight:bold

  class D,CH,EM1 ing
  class Q,EM2,SR,AU,AN qry
  class ST store
  class LLM llm
  linkStyle default stroke:#585b70,stroke-width:2px
```

Zwei Phasen: **Daten vorbereiten** (offline) und **Frage beantworten** (online).

---

# Schritt 1: Chunking

LLMs haben **Kontextfenster-Limits**, und kleinere Chunks geben **präziseres Retrieval**.
Wir zerteilen jedes Dokument in überlappende Stücke.

```mermaid
%%{init: {'theme': 'dark', 'themeVariables': {'primaryColor': '#313244', 'primaryTextColor': '#cdd6f4', 'lineColor': '#6c7086', 'fontSize': '15px'}}}%%
flowchart LR
  D["<b>Dokument</b>"]
  D --> C1["Chunk 1<br/>… <b>der Mahlgrad</b>"]
  D --> C2["Chunk 2<br/><b>Mahlgrad</b> … <b>9 Bar</b>"]
  D --> C3["Chunk 3<br/><b>9 Bar</b> … 25–30 Sek."]

  C1 -.-|"Überlappung"| C2
  C2 -.-|"Überlappung"| C3

  classDef d fill:#89b4fa,color:#1e1e2e,stroke:none,font-weight:bold
  classDef c fill:#313244,color:#cdd6f4,stroke:#a6e3a1,stroke-width:2px
  class D d
  class C1,C2,C3 c
  linkStyle 0,1,2 stroke:#585b70,stroke-width:2px
  linkStyle 3,4 stroke:#f9e2af,stroke-width:2px,stroke-dasharray:6
```

**Faustregeln:** Chunk-Größe **500–1000** Zeichen, **10–20% Überlappung**, an **Satzgrenzen** trennen.

---

# Schritt 2: Ingestion

<div class="columns">
<div class="col">

```csharp
// Pro Dokument einmal:
var chunks = _chunker.Chunk(document);

var embeddings =
  await _embedder.EmbedAsync(chunks);

await _vectorStore.UpsertAsync(
  chunks, embeddings);
```

Aus einer 50-Seiten-PDF werden so z. B.
**120 Chunks**, jeder mit eigenem Vektor.

</div>
<div class="col">

<div class="warning-box">

**⚠️ Niemals Embeddings verschiedener
Modelle in derselben Collection mischen!**

Jedes Modell hat seinen **eigenen
Vektorraum**. Cosinus zwischen
`nomic-embed-text` und
`text-embedding-3-small`
ergibt keinen Sinn.

**Konvention:** eine Collection pro Modell,
z. B. `documents_nomic-embed-text`.

</div>

</div>
</div>

---

# Schritt 3: Retrieve

Die Anfrage durchläuft **denselben Embedder** wie die Dokumente,
und Qdrant liefert die ähnlichsten Chunks zurück.

```csharp
// 1. Frage embedden
var queryVec = await _embedder.EmbedAsync(userQuery);

// 2. Top-K aus Qdrant holen (HNSW im Hintergrund)
var hits = await _collection.SearchAsync(queryVec, topK: 5);

// 3. optional: filtern, neu sortieren, Score-Schwelle anwenden
```

Typische Parameter: `TopK = 3–10`, Score-Schwelle `0.7–0.8`.

---

# Schritt 4: Augment

Wir bauen aus Frage + abgerufenen Chunks einen **erweiterten Prompt**.

```csharp
var prompt = $$"""
  Du bist ein hilfreicher Assistent. Beantworte die Frage
  AUSSCHLIESSLICH anhand des folgenden Kontexts.
  Wenn der Kontext nicht reicht, sage das ehrlich.

  KONTEXT:
  {{string.Join("\n---\n", retrievedChunks)}}

  FRAGE: {{userQuery}}
  """;
```

Wichtig: die **Regeln im System-Prompt** verhindern, dass das LLM
„aus dem Bauch" antwortet, wenn der Kontext nicht ausreicht.

---

# Schritt 5: Generate

Das LLM bekommt **Kontext + Frage** und formuliert eine Antwort
und idealerweise mit **Quellenangaben**.

<div class="success-box">

> *„Um Espresso zu machen, brauchst du **9 Bar Druck** und Wasser bei
> **90–96 °C**. Der Mahlgrad sollte extra-fein sein, die Extraktion
> dauert **25–30 Sekunden** für einen Single Shot."*
>
> *Quelle: `espresso-basics.md`*

</div>

Die Antwort ist **auf das gefundene Dokument gegroundet**, kein Halluzinieren mehr.

---

# Wichtige Parameter

| Parameter            | Beschreibung                         | Typischer Wert |
|----------------------|--------------------------------------|----------------|
| **Chunk-Größe**      | Zeichen pro Chunk                    | 500 – 1000     |
| **Überlappung**      | Gemeinsame Zeichen zwischen Chunks   | 50 – 200       |
| **Top-K**            | Anzahl abgerufener Chunks            | 3 – 10         |
| **Min. Score**       | Relevanz-Schwelle (Cosinus)          | 0.7 – 0.8      |
| **`efSearch` (HNSW)**| Recall vs. Speed                     | 32 – 256       |

Diese Werte sind **Stellschrauben**. Es gibt keinen „richtigen" Wert, nur Tradeoffs.

---

# Stolperstein 1: Lost in the Middle

LLMs achten **mehr auf Anfang und Ende** des Kontexts. Die Mitte wird häufig **ignoriert**.

<div class="columns">
<div class="col">

```text
 Performance
 100% ┤████                   ████
      │   ╲                 ╱
  50% ┤    ╲   ╭──╮  ╭──╮  ╱
      │     ╲ ╱    ╲╱    ╲╱
   0% ┴───────────────────────────►
       Anfang     Mitte       Ende
```

> *Liu et al., 2024:
> „Lost in the Middle:
> How Language Models
> Use Long Contexts"*

</div>
<div class="col">

### Lösungen

- **Top-K klein halten**
  (3–5 statt 20 Chunks)
- **Reranking** der Treffer
  (z. B. Cross-Encoder),
  wichtigste an den Rand
- **Zusammenfassen** statt
  alles komplett übergeben

</div>
</div>

---

# Stolperstein 2: Schlechtes Retrieval

Wenn der **richtige Chunk gar nicht gefunden** wird, hilft auch das beste LLM nicht.

<div class="columns">
<div class="col">

### Häufige Ursachen

- Synonyme / andere Sprache
- Negationen (*„ohne Hitze"*)
- Sehr kurze, vage Queries
- Fachbegriffe vs. Beschreibung

</div>
<div class="col">

### Mögliche Gegenmittel

- **Hybrid Search** (Vektor + Tags)
- **Query Expansion** (LLM erweitert die Frage)
- **HyDE**: hypothetisches Antwort­dokument embedden
- (Besseres Chunking)

</div>
</div>

> Das können wir bei der Demo live testen.

---

# Best Practices, kompakt

<div class="columns">
<div class="col">

### ✅ Tun

- **Eine Collection pro Embedding-Modell**
- Chunk-Größen experimentell finden
- Relevanz-Scores **loggen**
- Score-Schwelle setzen (`< 0.7` rausfiltern)
- Quellenangaben in die Antwort

</div>
<div class="col">

### ❌ Lassen

- Embeddings verschiedener Modelle mischen
- Riesen-Kontextfenster vollstopfen
- Score-Filterung weglassen
- LLM ohne klare Regeln antworten lassen
- Caching vergessen

</div>
</div>

---

# Take-Aways

<div class="info-box">

1. **RAG = Suche + LLM.** Das LLM bleibt unverändert, der Kontext kommt von außen.
2. **Vektorsuche** macht *Bedeutung* durchsuchbar, mit Embeddings und Cosinus.
3. **HNSW** ist der De-facto-Standard, damit Suche bei Millionen Vektoren funktioniert.
4. **Chunking, Top-K, Score-Schwelle** sind die wichtigsten Stellschrauben.
5. **Nie Embedding-Modelle mischen.**

</div>

---

# Ressourcen

### Demo-Repo
[github.com/AliMoinzadeh/Dojo.RAG](https://github.com/AliMoinzadeh/Dojo.RAG)

### Tools, die wir benutzt haben
- **Qdrant**: [qdrant.tech](https://qdrant.tech/documentation/)
- **Ollama** (lokale Modelle): [ollama.ai](https://ollama.ai/library)
- **LM Studio** (lokale Modelle): [lmstudio.ai](https://lmstudio.ai/)
- **Microsoft.Extensions.AI**: Provider-Abstraktion in .NET

---

<!-- _class: section -->

# Fragen?

## Dann zur Demo

---

# Demo-Drehbuch

1. **Dokumente ingesten**: `coffee-bean-origins.md` & Co. werden gechunkt und embedded
2. **Pipeline beobachten**: Embed-Time, Search-Time, Token-Usage live
3. **Vektorsuche pur**: *„Java Getränk"* scheitert mit reiner Vektorsuche
4. **Verbesserungen einschalten**: Query Expansion, Hybrid oder HyDE
5. **RAG-Chat**: *„Was unterscheidet Espresso von Pour Over?"*
6. **Mehrere Provider** zeigen: Ollama lokal gegen OpenAI

> Die Demo zeigt: **Vektorsuche allein reicht nicht immer.**
> Erst die Kombination der Techniken liefert robuste Ergebnisse.