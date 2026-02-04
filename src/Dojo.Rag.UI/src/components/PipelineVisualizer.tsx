import { FileText, Search, Sparkles, Cpu, CheckCircle } from 'lucide-react';
import type { PipelineMetrics } from '../types/api';

interface Props {
  metrics: PipelineMetrics;
}

const steps = [
  { name: 'Embed Query', icon: Cpu, key: 'embeddingTimeMs' as const },
  { name: 'Vector Search', icon: Search, key: 'searchTimeMs' as const },
  { name: 'LLM Generation', icon: Sparkles, key: 'generationTimeMs' as const },
];

export function PipelineVisualizer({ metrics }: Props) {
  return (
    <div className="bg-white p-4 rounded-lg border">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">RAG Pipeline</h4>
      
      {/* Pipeline steps */}
      <div className="flex items-center justify-between mb-4">
        {steps.map((step, index) => {
          const Icon = step.icon;
          const timeMs = metrics[step.key];
          
          return (
            <div key={step.name} className="flex items-center">
              <div className="flex flex-col items-center">
                <div className="w-12 h-12 rounded-full bg-green-100 flex items-center justify-center mb-1">
                  <Icon className="w-6 h-6 text-green-600" />
                </div>
                <span className="text-xs font-medium text-gray-700">{step.name}</span>
                <span className="text-xs text-gray-500">{timeMs}ms</span>
              </div>
              
              {index < steps.length - 1 && (
                <div className="w-12 h-0.5 bg-green-300 mx-2" />
              )}
            </div>
          );
        })}
        
        <div className="flex items-center">
          <div className="w-12 h-0.5 bg-green-300 mx-2" />
          <div className="flex flex-col items-center">
            <div className="w-12 h-12 rounded-full bg-green-500 flex items-center justify-center mb-1">
              <CheckCircle className="w-6 h-6 text-white" />
            </div>
            <span className="text-xs font-medium text-gray-700">Complete</span>
            <span className="text-xs text-gray-500">{metrics.totalTimeMs}ms</span>
          </div>
        </div>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 gap-4 text-sm">
        <div className="flex items-center gap-2">
          <FileText className="w-4 h-4 text-gray-400" />
          <span className="text-gray-600">
            Chunks: <strong>{metrics.chunksRetrieved}</strong>
          </span>
        </div>
        <div className="flex items-center gap-2">
          <Cpu className="w-4 h-4 text-gray-400" />
          <span className="text-gray-600">
            Embedding: <strong className="text-xs">{metrics.embeddingModel}</strong>
          </span>
        </div>
        <div className="flex items-center gap-2">
          <Sparkles className="w-4 h-4 text-gray-400" />
          <span className="text-gray-600">
            Chat: <strong className="text-xs">{metrics.chatModel}</strong>
          </span>
        </div>
      </div>
    </div>
  );
}
