import type {
  ChatRequest,
  ChatResponse,
  IngestRequest,
  IngestResponse,
  SourceDocument,
  CollectionInfo,
  ActiveConfig,
  EmbeddingVisualizationRequest,
  EmbeddingVisualizationResponse,
} from '../types/api';

const API_BASE = '/api';

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    throw new Error(errorData.error || `HTTP error! status: ${response.status}`);
  }
  return response.json();
}

export const api = {
  // Chat
  async chat(request: ChatRequest): Promise<ChatResponse> {
    const response = await fetch(`${API_BASE}/chat`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    });
    return handleResponse<ChatResponse>(response);
  },

  // Documents
  async ingestDocument(request: IngestRequest): Promise<IngestResponse> {
    const response = await fetch(`${API_BASE}/documents/ingest`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    });
    return handleResponse<IngestResponse>(response);
  },

  async getDocuments(): Promise<SourceDocument[]> {
    const response = await fetch(`${API_BASE}/documents`);
    return handleResponse<SourceDocument[]>(response);
  },

  async reIngestDocument(documentId: string): Promise<IngestResponse> {
    const response = await fetch(`${API_BASE}/documents/${documentId}/reingest`, {
      method: 'POST',
    });
    return handleResponse<IngestResponse>(response);
  },

  async deleteDocument(documentId: string): Promise<void> {
    const response = await fetch(`${API_BASE}/documents/${documentId}`, {
      method: 'DELETE',
    });
    await handleResponse<{ message: string }>(response);
  },

  async deleteAllDocuments(): Promise<void> {
    const response = await fetch(`${API_BASE}/documents`, {
      method: 'DELETE',
    });
    await handleResponse<{ message: string }>(response);
  },

  // Collections
  async getCollections(): Promise<CollectionInfo[]> {
    const response = await fetch(`${API_BASE}/collections`);
    return handleResponse<CollectionInfo[]>(response);
  },

  async getActiveConfig(): Promise<ActiveConfig> {
    const response = await fetch(`${API_BASE}/collections/active`);
    return handleResponse<ActiveConfig>(response);
  },

  async deleteCollection(collectionName: string): Promise<void> {
    const response = await fetch(`${API_BASE}/collections/${collectionName}`, {
      method: 'DELETE',
    });
    await handleResponse<{ message: string }>(response);
  },

  async deleteAllCollections(): Promise<void> {
    const response = await fetch(`${API_BASE}/collections`, {
      method: 'DELETE',
    });
    await handleResponse<{ message: string }>(response);
  },

  // Embeddings
  async visualizeEmbeddings(request: EmbeddingVisualizationRequest): Promise<EmbeddingVisualizationResponse> {
    const response = await fetch(`${API_BASE}/embeddings/visualize`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    });
    return handleResponse<EmbeddingVisualizationResponse>(response);
  },
};
