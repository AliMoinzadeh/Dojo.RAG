import { useState, useEffect } from 'react';
import { Search, Zap, RefreshCw, AlertCircle, CheckCircle, Lightbulb, ToggleLeft, ToggleRight } from 'lucide-react';
import { api } from '../lib/api';
import type {
  DemoSentencesResponse,
  VectorSearchDemoResponse,
  SearchResultItem,
  DemoStatus,
  SearchEnhancements,
} from '../types/api';

export function VectorSearchDemo() {
  const [sentences, setSentences] = useState<DemoSentencesResponse | null>(null);
  const [status, setStatus] = useState<DemoStatus | null>(null);
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<VectorSearchDemoResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [initializing, setInitializing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Enhancement toggles
  const [useHybridSearch, setUseHybridSearch] = useState(false);
  const [useQueryExpansion, setUseQueryExpansion] = useState(false);
  const [useReranking, setUseReranking] = useState(false);
  const [useMultiVectorSearch, setUseMultiVectorSearch] = useState(false);
  const [useContextualEmbeddings, setUseContextualEmbeddings] = useState(false);
  const [useHyDE, setUseHyDE] = useState(false);
  const [useHnswApproximate, setUseHnswApproximate] = useState(false);
  const [hnswEfSearch, setHnswEfSearch] = useState(32);
  const [minScore, setMinScore] = useState(0.5);

  useEffect(() => {
    loadSentencesAndStatus();
  }, []);

  const loadSentencesAndStatus = async () => {
    try {
      const [sentencesData, statusData] = await Promise.all([
        api.getDemoSentences(),
        api.getDemoStatus(),
      ]);
      setSentences(sentencesData);
      setStatus(statusData);
    } catch (err) {
      setError('Fehler beim Laden der Demo-Daten');
      console.error(err);
    }
  };

  const initializeDemo = async () => {
    setInitializing(true);
    setError(null);
    try {
      const result = await api.initializeDemo();
      if (result.success) {
        setStatus({
          isInitialized: true,
          embeddedSentenceCount: result.sentencesEmbedded,
          embeddingModel: 'current',
        });
      }
    } catch (err) {
      setError('Fehler beim Initialisieren der Demo');
      console.error(err);
    } finally {
      setInitializing(false);
    }
  };

  const handleSearch = async () => {
    if (!query.trim()) return;

    setLoading(true);
    setError(null);
    try {
      const enhancements: SearchEnhancements = {
        useHybridSearch,
        useQueryExpansion,
        useReranking,
        useMultiVectorSearch,
        useContextualEmbeddings,
        useHyDE,
        useHnswApproximate,
        hnswEfSearch,
      };

      const searchResults = await api.searchDemo({
        query: query.trim(),
        enhancements: (useHybridSearch || useQueryExpansion || useReranking || useMultiVectorSearch
          || useContextualEmbeddings || useHyDE || useHnswApproximate)
          ? enhancements
          : undefined,
        topK: 5,
        minScore,
      });
      setResults(searchResults);
    } catch (err) {
      setError('Fehler bei der Suche');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleScenarioClick = (scenarioQuery: string) => {
    setQuery(scenarioQuery);
  };

  const renderToggle = (
    label: string,
    description: string,
    value: boolean,
    onChange: (v: boolean) => void,
    icon: React.ReactNode
  ) => (
    <div
      className={`p-3 rounded-lg border cursor-pointer transition-all ${
        value
          ? 'bg-blue-900/30 border-blue-500'
          : 'bg-gray-800/50 border-gray-700 hover:border-gray-600'
      }`}
      onClick={() => onChange(!value)}
    >
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-2">
          {icon}
          <span className="font-medium text-white">{label}</span>
        </div>
        {value ? (
          <ToggleRight className="w-6 h-6 text-blue-400" />
        ) : (
          <ToggleLeft className="w-6 h-6 text-gray-500" />
        )}
      </div>
      <p className="text-xs text-gray-400 mt-1">{description}</p>
    </div>
  );

  const renderResultItem = (item: SearchResultItem, rank: number) => (
    <div
      key={item.id}
      className="p-3 bg-gray-800/50 rounded-lg border border-gray-700"
    >
      <div className="flex items-center justify-between mb-2">
        <span className="text-sm font-medium text-gray-400">#{rank}</span>
        <div className="flex items-center gap-2">
          <span className="text-xs px-2 py-0.5 bg-gray-700 rounded text-gray-300">
            {item.category}
          </span>
          <span
            className={`text-sm font-bold ${
              item.score >= 0.8
                ? 'text-green-400'
                : item.score >= 0.6
                ? 'text-yellow-400'
                : 'text-red-400'
            }`}
          >
            {(item.score * 100).toFixed(1)}%
          </span>
        </div>
      </div>
      <p className="text-white text-sm">{item.text}</p>
      {item.matchedKeywords.length > 0 && (
        <div className="mt-2 flex flex-wrap gap-1">
          {item.matchedKeywords.map((kw, idx) => (
            <span
              key={idx}
              className="text-xs px-1.5 py-0.5 bg-blue-900/50 text-blue-300 rounded"
            >
              {kw}
            </span>
          ))}
        </div>
      )}
    </div>
  );

  return (
    <div className="space-y-6">
      {/* Status and Initialize */}
      <div className="bg-gray-800 rounded-lg p-4 border border-gray-700">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            {status?.isInitialized ? (
              <CheckCircle className="w-5 h-5 text-green-400" />
            ) : (
              <AlertCircle className="w-5 h-5 text-yellow-400" />
            )}
            <div>
              <h3 className="text-white font-medium">Demo Status</h3>
              <p className="text-sm text-gray-400">
                {status?.isInitialized
                  ? `${status.embeddedSentenceCount} Sätze embedded`
                  : 'Nicht initialisiert - Embeddings müssen generiert werden'}
              </p>
            </div>
          </div>
          <button
            onClick={initializeDemo}
            disabled={initializing}
            className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-gray-600 rounded-lg text-white text-sm transition-colors"
          >
            <RefreshCw className={`w-4 h-4 ${initializing ? 'animate-spin' : ''}`} />
            {initializing ? 'Initialisiere...' : 'Initialisieren'}
          </button>
        </div>
      </div>

      {/* Enhancement Toggles */}
      <div className="bg-gray-800 rounded-lg p-4 border border-gray-700">
        <h3 className="text-white font-medium mb-3 flex items-center gap-2">
          <Zap className="w-4 h-4 text-yellow-400" />
          Such-Verbesserungen
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          {renderToggle(
            'Hybrid Search',
            'Kombiniert Vektor-Ähnlichkeit mit Keyword-Matching',
            useHybridSearch,
            setUseHybridSearch,
            <Search className="w-4 h-4 text-blue-400" />
          )}
          {renderToggle(
            'Query Expansion',
            'LLM erweitert Suchanfrage mit Synonymen',
            useQueryExpansion,
            setUseQueryExpansion,
            <Lightbulb className="w-4 h-4 text-yellow-400" />
          )}
          {renderToggle(
            'Reranking',
            'Cross-Encoder bewertet die Top-Ergebnisse neu',
            useReranking,
            setUseReranking,
            <Zap className="w-4 h-4 text-green-400" />
          )}
          {renderToggle(
            'Multi-Vector Search',
            'Separate Embeddings fuer Inhalt und Tags',
            useMultiVectorSearch,
            setUseMultiVectorSearch,
            <Search className="w-4 h-4 text-purple-400" />
          )}
          {renderToggle(
            'Contextual Embeddings',
            'Einbettung mit Dokumentkontext (Kategorie, Tags)',
            useContextualEmbeddings,
            setUseContextualEmbeddings,
            <Lightbulb className="w-4 h-4 text-orange-400" />
          )}
          {renderToggle(
            'HyDE',
            'LLM erzeugt hypothetisches Dokument fuer die Embedding-Suche',
            useHyDE,
            setUseHyDE,
            <Zap className="w-4 h-4 text-cyan-400" />
          )}
          {renderToggle(
            'HNSW Approx',
            'Approximate Search fuer schnellere Suche (Demo-Simulation)',
            useHnswApproximate,
            setUseHnswApproximate,
            <Search className="w-4 h-4 text-emerald-400" />
          )}
        </div>
        {useHnswApproximate && (
          <div className="mt-3">
            <div className="flex items-center justify-between mb-1">
              <span className="text-sm text-gray-300">HNSW efSearch (Kandidaten)</span>
              <span className="text-xs text-gray-400">{hnswEfSearch}</span>
            </div>
            <input
              type="range"
              min={5}
              max={50}
              step={1}
              value={hnswEfSearch}
              onChange={(e) => setHnswEfSearch(parseInt(e.target.value, 10))}
              className="w-full accent-emerald-500"
            />
          </div>
        )}
      </div>

      {/* Search Input */}
      <div className="bg-gray-800 rounded-lg p-4 border border-gray-700">
        <div className="flex gap-3">
          <input
            type="text"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
            placeholder="Suchanfrage eingeben..."
            className="flex-1 px-4 py-2 bg-gray-900 border border-gray-600 rounded-lg text-white placeholder-gray-500 focus:outline-none focus:border-blue-500"
          />
          <button
            onClick={handleSearch}
            disabled={loading || !status?.isInitialized}
            className="flex items-center gap-2 px-6 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-gray-600 rounded-lg text-white transition-colors"
          >
            <Search className={`w-4 h-4 ${loading ? 'animate-pulse' : ''}`} />
            Suchen
          </button>
        </div>

        <div className="mt-4">
          <div className="flex items-center justify-between mb-1">
            <span className="text-sm text-gray-300">Min-Score Threshold</span>
            <span className="text-xs text-gray-400">{minScore.toFixed(2)}</span>
          </div>
          <input
            type="range"
            min={0}
            max={1}
            step={0.01}
            value={minScore}
            onChange={(e) => setMinScore(parseFloat(e.target.value))}
            className="w-full accent-blue-500"
          />
        </div>

        {/* Demo Scenarios */}
        {sentences?.demoScenarios && (
          <div className="mt-4">
            <p className="text-sm text-gray-400 mb-2">Beispiel-Szenarien (Limitationen):</p>
            <div className="flex flex-wrap gap-2">
              {sentences.demoScenarios.map((scenario) => (
                <button
                  key={scenario.name}
                  onClick={() => handleScenarioClick(scenario.query)}
                  className="px-3 py-1.5 text-xs bg-gray-700 hover:bg-gray-600 text-gray-300 rounded-lg transition-colors"
                  title={scenario.explanation}
                >
                  {scenario.name}: "{scenario.query}"
                </button>
              ))}
            </div>
          </div>
        )}
      </div>

      {/* Error Display */}
      {error && (
        <div className="bg-red-900/30 border border-red-600 rounded-lg p-4 text-red-400">
          {error}
        </div>
      )}

      {/* Results Side-by-Side */}
      {results && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {/* Standard Results */}
          <div className="bg-gray-800 rounded-lg p-4 border border-gray-700">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-white font-medium">Standard Vektorsuche</h3>
              <span className="text-xs text-gray-400">
                {results.standardResults.searchTimeMs}ms
              </span>
            </div>
            <div className="space-y-3">
              {results.standardResults.results.map((item, idx) =>
                renderResultItem(item, idx + 1)
              )}
              {results.standardResults.results.length === 0 && (
                <p className="text-gray-500 text-sm">Keine Ergebnisse</p>
              )}
            </div>
          </div>

          {/* Enhanced Results */}
          <div
            className={`bg-gray-800 rounded-lg p-4 border ${
              results.enhancedResults
                ? 'border-blue-500'
                : 'border-gray-700 opacity-50'
            }`}
          >
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-white font-medium flex items-center gap-2">
                <Zap className="w-4 h-4 text-yellow-400" />
                Mit Verbesserungen
              </h3>
              {results.enhancedResults && (
                <span className="text-xs text-gray-400">
                  {results.enhancedResults.searchTimeMs}ms
                </span>
              )}
            </div>

            {results.enhancedResults ? (
              <>
                {results.enhancedResults.expandedQuery && (
                  <div className="mb-3 p-2 bg-blue-900/20 border border-blue-800 rounded text-xs">
                    <span className="text-blue-400">Erweiterte Query: </span>
                    <span className="text-blue-200">
                      {results.enhancedResults.expandedQuery}
                    </span>
                  </div>
                )}
                {results.enhancedResults.hypotheticalDocument && (
                  <div className="mb-3 p-2 bg-cyan-900/20 border border-cyan-800 rounded text-xs">
                    <span className="text-cyan-300">HyDE Dokument: </span>
                    <span className="text-cyan-100">
                      {results.enhancedResults.hypotheticalDocument}
                    </span>
                  </div>
                )}
                <div className="space-y-3">
                  {results.enhancedResults.results.map((item, idx) =>
                    renderResultItem(item, idx + 1)
                  )}
                  {results.enhancedResults.results.length === 0 && (
                    <p className="text-gray-500 text-sm">Keine Ergebnisse</p>
                  )}
                </div>
              </>
            ) : (
              <p className="text-gray-500 text-sm">
                Aktiviere mindestens eine Verbesserung für den Vergleich
              </p>
            )}
          </div>
        </div>
      )}

      {/* Sentence Reference */}
      {sentences && (
        <div className="bg-gray-800 rounded-lg p-4 border border-gray-700">
          <h3 className="text-white font-medium mb-3">
            Verfügbare Demo-Sätze ({sentences.sentences.length})
          </h3>
          <div className="max-h-60 overflow-y-auto space-y-2">
            {sentences.sentences.map((sentence) => (
              <div
                key={sentence.id}
                className="p-2 bg-gray-900/50 rounded text-sm border border-gray-800"
              >
                <div className="flex items-center gap-2 mb-1">
                  <span className="text-xs px-1.5 py-0.5 bg-gray-700 rounded text-gray-400">
                    {sentence.category}
                  </span>
                </div>
                <p className="text-gray-300">{sentence.text}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Educational Info */}
      <div className="bg-yellow-900/30 border border-yellow-600/50 rounded-lg p-4">
        <h3 className="text-lg font-semibold text-yellow-400 mb-2 flex items-center gap-2">
          <Lightbulb className="w-5 h-5" />
          Über diese Demo
        </h3>
        <div className="text-sm text-yellow-200/80 space-y-2">
          <p>
            <strong>Ziel:</strong> Zeigt Limitationen der reinen Vektorsuche und wie
            verschiedene Techniken diese verbessern können.
          </p>
          <p>
            <strong>Hybrid Search:</strong> Kombiniert semantische Vektorähnlichkeit
            mit exaktem Keyword-Matching. Hilft bei Fachbegriffen und exakten Wörtern.
          </p>
          <p>
            <strong>Query Expansion:</strong> Ein LLM erweitert die Suchanfrage mit
            Synonymen und verwandten Begriffen, bevor die Vektorsuche ausgeführt wird.
          </p>
          <p>
            <strong>Reranking:</strong> Ein Cross-Encoder bewertet die Top-Ergebnisse
            neu und ordnet sie nach feinerer Relevanz.
          </p>
          <p>
            <strong>Multi-Vector Search:</strong> Inhalt und Tags werden getrennt
            eingebettet und gewichtet zusammengefuehrt.
          </p>
          <p>
            <strong>Contextual Embeddings:</strong> Chunks werden mit Kontext wie
            Kategorie und Tags eingebettet, um mehr Kontext zu liefern.
          </p>
          <p>
            <strong>HyDE:</strong> Ein LLM erzeugt ein hypothetisches Dokument, das
            embedded wird, um bessere Matches fuer kurze Anfragen zu finden.
          </p>
          <p>
            <strong>HNSW Approx:</strong> Approximate Search reduziert die Kandidaten
            fuer schnellere Suche (hier als Demo-Simulation).
          </p>
        </div>
      </div>
    </div>
  );
}
