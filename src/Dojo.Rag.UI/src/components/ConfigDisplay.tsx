import { useState, useEffect } from 'react';
import { Settings, Server, Database, Cpu } from 'lucide-react';
import type { ActiveConfig } from '../types/api';
import { api } from '../lib/api';

export function ConfigDisplay() {
  const [config, setConfig] = useState<ActiveConfig | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadConfig();
  }, []);

  const loadConfig = async () => {
    try {
      const data = await api.getActiveConfig();
      setConfig(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load config');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <div className="bg-white rounded-lg shadow-lg p-4">
        <div className="animate-pulse space-y-2">
          <div className="h-4 bg-gray-200 rounded w-1/2"></div>
          <div className="h-3 bg-gray-200 rounded w-3/4"></div>
          <div className="h-3 bg-gray-200 rounded w-2/3"></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-white rounded-lg shadow-lg p-4">
        <div className="text-red-600 text-sm">{error}</div>
        <button 
          onClick={loadConfig}
          className="mt-2 text-blue-600 text-sm hover:underline"
        >
          Retry
        </button>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-lg shadow-lg p-4">
      <div className="flex items-center gap-2 mb-4">
        <Settings className="w-5 h-5 text-gray-600" />
        <h3 className="text-lg font-semibold text-gray-800">Active Configuration</h3>
      </div>

      {config && (
        <div className="space-y-3">
          <div className="flex items-center gap-3 p-2 bg-gray-50 rounded-lg">
            <Server className="w-5 h-5 text-blue-500" />
            <div>
              <p className="text-sm font-medium text-gray-700">Provider</p>
              <p className="text-sm text-gray-600">{config.provider}</p>
            </div>
          </div>

          <div className="flex items-center gap-3 p-2 bg-gray-50 rounded-lg">
            <Cpu className="w-5 h-5 text-purple-500" />
            <div>
              <p className="text-sm font-medium text-gray-700">Chat Model</p>
              <p className="text-sm text-gray-600">{config.chatModel}</p>
            </div>
          </div>

          <div className="flex items-center gap-3 p-2 bg-gray-50 rounded-lg">
            <Cpu className="w-5 h-5 text-green-500" />
            <div>
              <p className="text-sm font-medium text-gray-700">Embedding Model</p>
              <p className="text-sm text-gray-600">
                {config.embeddingModel} ({config.embeddingDimensions}d)
              </p>
            </div>
          </div>

          <div className="flex items-center gap-3 p-2 bg-gray-50 rounded-lg">
            <Database className="w-5 h-5 text-orange-500" />
            <div>
              <p className="text-sm font-medium text-gray-700">Collection</p>
              <p className="text-sm text-gray-600 text-xs break-all">{config.collectionName}</p>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
