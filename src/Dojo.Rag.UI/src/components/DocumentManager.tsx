import { useState, useEffect, useCallback } from 'react';
import { FileText, Trash2, RefreshCw, AlertCircle, Database, Loader2 } from 'lucide-react';
import { api } from '../lib/api';
import type { SourceDocument, CollectionInfo, ActiveConfig } from '../types/api';

export function DocumentManager() {
  const [documents, setDocuments] = useState<SourceDocument[]>([]);
  const [collections, setCollections] = useState<CollectionInfo[]>([]);
  const [activeConfig, setActiveConfig] = useState<ActiveConfig | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [actionInProgress, setActionInProgress] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const [docs, cols, config] = await Promise.all([
        api.getDocuments(),
        api.getCollections(),
        api.getActiveConfig(),
      ]);
      setDocuments(docs);
      setCollections(cols);
      setActiveConfig(config);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch data');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const handleDeleteDocument = async (docId: string) => {
    if (!confirm('Delete this document? This will NOT remove it from vector collections.')) return;
    
    try {
      setActionInProgress(`delete-doc-${docId}`);
      await api.deleteDocument(docId);
      await fetchData();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete document');
    } finally {
      setActionInProgress(null);
    }
  };

  const handleDeleteAllDocuments = async () => {
    if (!confirm('Delete ALL documents and ALL vector collections? This cannot be undone!')) return;
    
    try {
      setActionInProgress('delete-all');
      await api.deleteAllDocuments();
      await fetchData();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete all documents');
    } finally {
      setActionInProgress(null);
    }
  };

  const handleDeleteCollection = async (collectionName: string) => {
    if (!confirm(`Delete collection "${collectionName}"? This will remove all vectors in this collection.`)) return;
    
    try {
      setActionInProgress(`delete-col-${collectionName}`);
      await api.deleteCollection(collectionName);
      await fetchData();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete collection');
    } finally {
      setActionInProgress(null);
    }
  };

  const handleReIngest = async (docId: string) => {
    try {
      setActionInProgress(`reingest-${docId}`);
      await api.reIngestDocument(docId);
      await fetchData();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to re-ingest document');
    } finally {
      setActionInProgress(null);
    }
  };

  if (loading) {
    return (
      <div className="bg-gray-800 rounded-lg p-4 flex items-center justify-center">
        <Loader2 className="h-6 w-6 animate-spin text-blue-400" />
        <span className="ml-2 text-gray-300">Loading...</span>
      </div>
    );
  }

  return (
    <div className="bg-gray-800 rounded-lg p-4 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <h2 className="text-lg font-semibold text-white flex items-center gap-2">
          <Database className="h-5 w-5 text-blue-400" />
          Document Manager
        </h2>
        <div className="flex gap-2">
          <button
            onClick={fetchData}
            disabled={!!actionInProgress}
            className="px-3 py-1 text-sm bg-gray-700 hover:bg-gray-600 text-white rounded flex items-center gap-1"
          >
            <RefreshCw className="h-4 w-4" />
            Refresh
          </button>
          <button
            onClick={handleDeleteAllDocuments}
            disabled={!!actionInProgress || documents.length === 0}
            className="px-3 py-1 text-sm bg-red-600 hover:bg-red-700 disabled:bg-gray-600 text-white rounded flex items-center gap-1"
          >
            <Trash2 className="h-4 w-4" />
            Delete All
          </button>
        </div>
      </div>

      {/* Error Display */}
      {error && (
        <div className="bg-red-900/50 border border-red-500 rounded p-3 flex items-center gap-2 text-red-200">
          <AlertCircle className="h-5 w-5" />
          {error}
          <button onClick={() => setError(null)} className="ml-auto text-red-400 hover:text-red-200">×</button>
        </div>
      )}

      {/* Active Config */}
      {activeConfig && (
        <div className="bg-gray-700/50 rounded p-3 text-sm">
          <div className="text-gray-400 mb-1">Active Configuration</div>
          <div className="grid grid-cols-2 gap-2 text-gray-200">
            <div><span className="text-gray-500">Provider:</span> {activeConfig.provider}</div>
            <div><span className="text-gray-500">Chat:</span> {activeConfig.chatModel}</div>
            <div><span className="text-gray-500">Embedding:</span> {activeConfig.embeddingModel}</div>
            <div><span className="text-gray-500">Collection:</span> {activeConfig.collectionName}</div>
          </div>
        </div>
      )}

      {/* Source Documents */}
      <div>
        <h3 className="text-md font-medium text-gray-300 mb-2">
          Source Documents ({documents.length})
        </h3>
        {documents.length === 0 ? (
          <div className="text-gray-500 text-sm italic">No documents uploaded yet.</div>
        ) : (
          <div className="space-y-2 max-h-64 overflow-y-auto">
            {documents.map((doc) => (
              <div
                key={doc.id}
                className="bg-gray-700 rounded p-3 flex items-center justify-between"
              >
                <div className="flex items-center gap-2 min-w-0">
                  <FileText className="h-4 w-4 text-blue-400 flex-shrink-0" />
                  <div className="min-w-0">
                    <div className="text-white truncate">{doc.fileName}</div>
                    <div className="text-xs text-gray-400">
                      ID: {doc.id.substring(0, 8)}... | {doc.content.length.toLocaleString()} chars
                    </div>
                  </div>
                </div>
                <div className="flex gap-1 flex-shrink-0">
                  <button
                    onClick={() => handleReIngest(doc.id)}
                    disabled={!!actionInProgress}
                    className="p-1 text-blue-400 hover:bg-blue-600/20 rounded"
                    title="Re-ingest with current embedding model"
                  >
                    {actionInProgress === `reingest-${doc.id}` ? (
                      <Loader2 className="h-4 w-4 animate-spin" />
                    ) : (
                      <RefreshCw className="h-4 w-4" />
                    )}
                  </button>
                  <button
                    onClick={() => handleDeleteDocument(doc.id)}
                    disabled={!!actionInProgress}
                    className="p-1 text-red-400 hover:bg-red-600/20 rounded"
                    title="Delete source document"
                  >
                    {actionInProgress === `delete-doc-${doc.id}` ? (
                      <Loader2 className="h-4 w-4 animate-spin" />
                    ) : (
                      <Trash2 className="h-4 w-4" />
                    )}
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Vector Collections */}
      <div>
        <h3 className="text-md font-medium text-gray-300 mb-2">
          Vector Collections ({collections.length})
        </h3>
        {collections.length === 0 ? (
          <div className="text-gray-500 text-sm italic">No collections created yet.</div>
        ) : (
          <div className="space-y-2 max-h-48 overflow-y-auto">
            {collections.map((col) => (
              <div
                key={col.name}
                className={`rounded p-3 flex items-center justify-between ${
                  col.isActive ? 'bg-blue-900/30 border border-blue-500/50' : 'bg-gray-700'
                }`}
              >
                <div className="min-w-0">
                  <div className="flex items-center gap-2">
                    <span className="text-white truncate">{col.name}</span>
                    {col.isActive && (
                      <span className="px-2 py-0.5 text-xs bg-blue-600 text-white rounded">Active</span>
                    )}
                  </div>
                  <div className="text-xs text-gray-400">
                    Model: {col.embeddingModel} | {col.dimensions}D
                  </div>
                </div>
                <button
                  onClick={() => handleDeleteCollection(col.name)}
                  disabled={!!actionInProgress}
                  className="p-1 text-red-400 hover:bg-red-600/20 rounded flex-shrink-0"
                  title="Delete collection"
                >
                  {actionInProgress === `delete-col-${col.name}` ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : (
                    <Trash2 className="h-4 w-4" />
                  )}
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
