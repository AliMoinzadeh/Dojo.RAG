import { useState } from 'react';
import { Send, Loader2, ChevronDown, ChevronUp } from 'lucide-react';
import type { ChatResponse, RetrievedChunk, TokenUsage, PipelineMetrics, RagSearchEnhancements } from '../types/api';
import { api } from '../lib/api';
import { ContextWindowGauge } from './ContextWindowGauge';
import { RelevanceScoreDisplay } from './RelevanceScoreDisplay';
import { PipelineVisualizer } from './PipelineVisualizer';

interface Message {
  role: 'user' | 'assistant';
  content: string;
  chunks?: RetrievedChunk[];
  tokenUsage?: TokenUsage;
  metrics?: PipelineMetrics;
}

type SearchEnhancementOption =
  | 'none'
  | 'hybrid'
  | 'queryExpansion'
  | 'reranking'
  | 'hyde'
  | 'hnswApproximate';

export function ChatWindow() {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [expandedMessageId, setExpandedMessageId] = useState<number | null>(null);
  const [includeDebugInfo, setIncludeDebugInfo] = useState(true);
  const [selectedEnhancement, setSelectedEnhancement] = useState<SearchEnhancementOption>('none');
  const [hnswEfSearch, setHnswEfSearch] = useState(32);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!input.trim() || isLoading) return;

    const userMessage: Message = { role: 'user', content: input };
    setMessages(prev => [...prev, userMessage]);
    setInput('');
    setIsLoading(true);
    setError(null);

    try {
      const enhancements: RagSearchEnhancements | undefined = selectedEnhancement !== 'none'
        ? {
            useHybridSearch: selectedEnhancement === 'hybrid',
            useQueryExpansion: selectedEnhancement === 'queryExpansion',
            useReranking: selectedEnhancement === 'reranking',
            useHyDE: selectedEnhancement === 'hyde',
            useHnswApproximate: selectedEnhancement === 'hnswApproximate',
            hnswEfSearch,
          }
        : undefined;

      const response: ChatResponse = await api.chat({
        message: input,
        includeDebugInfo,
        enhancements,
      });

      const assistantMessage: Message = {
        role: 'assistant',
        content: response.answer,
        chunks: response.retrievedChunks,
        tokenUsage: response.tokenUsage,
        metrics: response.metrics,
      };

      setMessages(prev => [...prev, assistantMessage]);
      setExpandedMessageId(messages.length + 1); // Auto-expand latest response
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setIsLoading(false);
    }
  };

  const toggleExpanded = (index: number) => {
    setExpandedMessageId(expandedMessageId === index ? null : index);
  };

  return (
    <div className="flex flex-col h-full bg-white rounded-lg shadow-lg">
      {/* Header */}
      <div className="px-4 py-3 border-b bg-gradient-to-r from-blue-600 to-purple-600 rounded-t-lg">
        <h2 className="text-lg font-semibold text-white">RAG Chat</h2>
        <p className="text-sm text-blue-100">Ask questions about your documents</p>
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto p-4 space-y-4">
        {messages.length === 0 && (
          <div className="text-center text-gray-500 py-8">
            <p className="text-lg">No messages yet</p>
            <p className="text-sm">Upload some documents and start asking questions!</p>
          </div>
        )}

        {messages.map((message, index) => (
          <div
            key={index}
            className={`flex ${message.role === 'user' ? 'justify-end' : 'justify-start'}`}
          >
            <div
              className={`max-w-[80%] rounded-lg p-4 ${
                message.role === 'user'
                  ? 'bg-blue-600 text-white'
                  : 'bg-gray-100 text-gray-800'
              }`}
            >
              <p className="whitespace-pre-wrap">{message.content}</p>

              {/* RAG Details for assistant messages */}
              {message.role === 'assistant' && (message.chunks || message.metrics) && (
                <div className="mt-3">
                  <button
                    onClick={() => toggleExpanded(index)}
                    className="flex items-center gap-1 text-sm text-blue-600 hover:text-blue-800"
                  >
                    {expandedMessageId === index ? (
                      <>
                        <ChevronUp size={16} /> Hide RAG Details
                      </>
                    ) : (
                      <>
                        <ChevronDown size={16} /> Show RAG Details
                      </>
                    )}
                  </button>

                  {expandedMessageId === index && (
                    <div className="mt-3 space-y-4 border-t pt-3">
                      {/* Pipeline Metrics */}
                      {message.metrics && (
                        <PipelineVisualizer metrics={message.metrics} />
                      )}

                      {/* Token Usage */}
                      {message.tokenUsage && (
                        <ContextWindowGauge usage={message.tokenUsage} />
                      )}

                      {/* Retrieved Chunks */}
                      {message.chunks && message.chunks.length > 0 && (
                        <RelevanceScoreDisplay chunks={message.chunks} />
                      )}
                    </div>
                  )}
                </div>
              )}
            </div>
          </div>
        ))}

        {isLoading && (
          <div className="flex justify-start">
            <div className="bg-gray-100 rounded-lg p-4 flex items-center gap-2">
              <Loader2 className="animate-spin" size={20} />
              <span>Thinking...</span>
            </div>
          </div>
        )}

        {error && (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        )}
      </div>

      {/* Input */}
      <form onSubmit={handleSubmit} className="p-4 border-t">
        <div className="flex items-center gap-2 mb-2">
          <label className="flex items-center gap-2 text-sm text-gray-600">
            <input
              type="checkbox"
              checked={includeDebugInfo}
              onChange={(e) => setIncludeDebugInfo(e.target.checked)}
              className="rounded"
            />
            Include debug info
          </label>
        </div>
        <div className="mb-3">
          <div className="text-sm font-medium text-gray-700 mb-2">Search enhancements</div>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 text-sm text-gray-600">
            <label className="flex items-center gap-2">
              <input
                type="radio"
                name="searchEnhancement"
                value="none"
                checked={selectedEnhancement === 'none'}
                onChange={() => setSelectedEnhancement('none')}
              />
              None
            </label>
            <label className="flex items-center gap-2">
              <input
                type="radio"
                name="searchEnhancement"
                value="hybrid"
                checked={selectedEnhancement === 'hybrid'}
                onChange={() => setSelectedEnhancement('hybrid')}
              />
              Hybrid search
            </label>
            <label className="flex items-center gap-2">
              <input
                type="radio"
                name="searchEnhancement"
                value="queryExpansion"
                checked={selectedEnhancement === 'queryExpansion'}
                onChange={() => setSelectedEnhancement('queryExpansion')}
              />
              Query expansion
            </label>
            <label className="flex items-center gap-2">
              <input
                type="radio"
                name="searchEnhancement"
                value="reranking"
                checked={selectedEnhancement === 'reranking'}
                onChange={() => setSelectedEnhancement('reranking')}
              />
              Reranking
            </label>
            <label className="flex items-center gap-2">
              <input
                type="radio"
                name="searchEnhancement"
                value="hyde"
                checked={selectedEnhancement === 'hyde'}
                onChange={() => setSelectedEnhancement('hyde')}
              />
              HyDE
            </label>
            <label className="flex items-center gap-2">
              <input
                type="radio"
                name="searchEnhancement"
                value="hnswApproximate"
                checked={selectedEnhancement === 'hnswApproximate'}
                onChange={() => setSelectedEnhancement('hnswApproximate')}
              />
              HNSW approximate
            </label>
          </div>
          {selectedEnhancement === 'hnswApproximate' && (
            <div className="mt-2">
              <div className="flex items-center justify-between text-xs text-gray-500 mb-1">
                <span>HNSW efSearch</span>
                <span>{hnswEfSearch}</span>
              </div>
              <input
                type="range"
                min={5}
                max={80}
                step={1}
                value={hnswEfSearch}
                onChange={(e) => setHnswEfSearch(parseInt(e.target.value, 10))}
                className="w-full accent-blue-500"
              />
            </div>
          )}
        </div>
        <div className="flex gap-2">
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            placeholder="Ask a question about your documents..."
            className="flex-1 px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            disabled={isLoading}
          />
          <button
            type="submit"
            disabled={isLoading || !input.trim()}
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Send size={20} />
          </button>
        </div>
      </form>
    </div>
  );
}
