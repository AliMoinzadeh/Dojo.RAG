import { useState, useCallback } from 'react';
import { Upload, FileText, Loader2, CheckCircle } from 'lucide-react';
import type { IngestResponse } from '../types/api';
import { api } from '../lib/api';

interface Props {
  onIngestComplete?: (response: IngestResponse) => void;
}

export function DocumentUpload({ onIngestComplete }: Props) {
  const [isLoading, setIsLoading] = useState(false);
  const [dragActive, setDragActive] = useState(false);
  const [lastResult, setLastResult] = useState<IngestResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleFile = async (file: File) => {
    if (!file.name.endsWith('.txt') && !file.name.endsWith('.md')) {
      setError('Only .txt and .md files are supported');
      return;
    }

    setIsLoading(true);
    setError(null);
    setLastResult(null);

    try {
      const content = await file.text();
      const response = await api.ingestDocument({
        fileName: file.name,
        content,
      });
      
      setLastResult(response);
      onIngestComplete?.(response);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to ingest document');
    } finally {
      setIsLoading(false);
    }
  };

  const handleDrag = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.type === 'dragenter' || e.type === 'dragover') {
      setDragActive(true);
    } else if (e.type === 'dragleave') {
      setDragActive(false);
    }
  }, []);

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setDragActive(false);
    
    if (e.dataTransfer.files && e.dataTransfer.files[0]) {
      handleFile(e.dataTransfer.files[0]);
    }
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    e.preventDefault();
    if (e.target.files && e.target.files[0]) {
      handleFile(e.target.files[0]);
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-lg p-4">
      <h3 className="text-lg font-semibold text-gray-800 mb-4">Document Upload</h3>
      
      <div
        className={`border-2 border-dashed rounded-lg p-8 text-center transition-colors ${
          dragActive
            ? 'border-blue-500 bg-blue-50'
            : 'border-gray-300 hover:border-gray-400'
        }`}
        onDragEnter={handleDrag}
        onDragLeave={handleDrag}
        onDragOver={handleDrag}
        onDrop={handleDrop}
      >
        {isLoading ? (
          <div className="flex flex-col items-center gap-2">
            <Loader2 className="w-10 h-10 text-blue-500 animate-spin" />
            <p className="text-gray-600">Processing document...</p>
          </div>
        ) : (
          <label className="cursor-pointer flex flex-col items-center gap-2">
            <Upload className="w-10 h-10 text-gray-400" />
            <p className="text-gray-600">
              Drag & drop a file here, or <span className="text-blue-600">browse</span>
            </p>
            <p className="text-sm text-gray-400">Supports .txt and .md files</p>
            <input
              type="file"
              accept=".txt,.md"
              onChange={handleChange}
              className="hidden"
            />
          </label>
        )}
      </div>

      {error && (
        <div className="mt-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
          {error}
        </div>
      )}

      {lastResult && (
        <div className="mt-4 p-3 bg-green-50 border border-green-200 rounded-lg">
          <div className="flex items-center gap-2 text-green-700 mb-2">
            <CheckCircle className="w-5 h-5" />
            <span className="font-medium">Document ingested successfully!</span>
          </div>
          <div className="text-sm text-green-600 space-y-1">
            <p className="flex items-center gap-2">
              <FileText className="w-4 h-4" />
              {lastResult.fileName}
            </p>
            <p>Chunks created: {lastResult.chunksCreated}</p>
            <p>Collection: {lastResult.collectionName}</p>
            <p>Processing time: {lastResult.processingTimeMs}ms</p>
          </div>
        </div>
      )}
    </div>
  );
}
