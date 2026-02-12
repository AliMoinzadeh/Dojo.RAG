## Erklärung: Distanzfunktionen und Cosinus-Ähnlichkeit

### 1. Distanzfunktionen (Zeilen 231-237)

Die drei Metriken im Vergleich:

| Metrik | Was sie misst | Wann verwenden? |
|--------|---------------|-----------------|
| **Cosinus** | Winkel zwischen Vektoren | Text-Embeddings, semantische Ähnlichkeit (ignoriert die Länge) |
| **Euklidisch** | Geradlinige Distanz | Absolute Abstände wichtig (z.B. Clustering, GPS) |
| **Skalarprodukt** | Winkel + Länge | Wenn Größenordnung relevant ist (z.B. Ranking, Empfehlungen) |

**Cosinus-Ähnlichkeit ist am häufigsten**, weil:
- Moderne Embeddings bereits normalisiert sind (Länge = 1)
- Bei Text interessiert uns die **Bedeutungsrichtung**, nicht die Dokumentlänge
- Eine kurze Query und ein langes Dokument können semantisch identisch sein

---

### 2. Cosinus-Ähnlichkeit (Zeilen 239-245)

**Formel:**
```
sim(A, B) = (A · B) / (||A|| × ||B||)
```

**Bedeutung:**
- **Zähler (A · B)**: Skalarprodukt der beiden Vektoren
- **Nenner (||A|| × ||B||)**: Produkt der Vektorlängen (Normalisierung)

**Wertebereich -1 bis 1:**
- **1** = Identische Richtung (perfekte Übereinstimmung)
- **0** = Orthogonal (keine Korrelation)
- **-1** = Entgegengesetzte Richtung (perfekte Gegenteiligkeit)

**Beispiel:**
```
"Kaffee brühen"     → [0.12, -0.45, 0.78, ...]
"Espresso machen"   → [0.14, -0.42, 0.81, ...]  ← Cosinus-Ähnlichkeit ≈ 0.95
"Auto Motor"        → [-0.67, 0.32, -0.15, ...] ← Cosinus-Ähnlichkeit ≈ -0.2
```

Die Cosinus-Ähnlichkeit ist ideal für RAG-Systeme, da sie semantische Bedeutung erfasst, unabhängig von der Textlänge, und extrem effizient zu berechnen ist.