// API Types matching the .NET backend models

export interface ChatRequest {
  message: string;
  includeDebugInfo?: boolean;
}

export interface ChatResponse {
  answer: string;
  retrievedChunks: RetrievedChunk[];
  tokenUsage?: TokenUsage;
  metrics?: PipelineMetrics;
}

export interface RetrievedChunk {
  id: string;
  content: string;
  sourceFileName: string;
  chunkIndex: number;
  relevanceScore: number;
}

export interface TokenUsage {
  systemPromptTokens: number;
  contextTokens: number;
  queryTokens: number;
  totalInputTokens: number;
  maxContextTokens: number;
  usagePercentage: number;
}

export interface PipelineMetrics {
  embeddingTimeMs: number;
  searchTimeMs: number;
  generationTimeMs: number;
  totalTimeMs: number;
  chunksRetrieved: number;
  embeddingModel: string;
  chatModel: string;
}

export interface IngestRequest {
  fileName: string;
  content: string;
}

export interface IngestResponse {
  documentId: string;
  fileName: string;
  chunksCreated: number;
  collectionName: string;
  processingTimeMs: number;
}

export interface SourceDocument {
  id: string;
  fileName: string;
  content: string;
  uploadedAt: string;
}

export interface CollectionInfo {
  name: string;
  embeddingModel: string;
  dimensions: number;
  documentCount: number;
  isActive: boolean;
}

export interface ActiveConfig {
  provider: string;
  chatModel: string;
  embeddingModel: string;
  embeddingDimensions: number;
  collectionName: string;
}

export interface EmbeddingVisualizationRequest {
  query?: string;
  maxPoints?: number;
}

export interface EmbeddingPoint {
  id: string;
  textPreview: string;
  sourceFile: string;
  x: number;
  y: number;
  isQuery: boolean;
  relevanceScore?: number;
}

export interface EmbeddingVisualizationResponse {
  points: EmbeddingPoint[];
  collectionName: string;
  embeddingModel: string;
  originalDimensions: number;
}

// Vector Search Demo Types
export interface DemoSentence {
  id: string;
  text: string;
  category: string;
  tags: string[];
}

export interface DemoScenario {
  name: string;
  query: string;
  expectedMatch: string;
  explanation: string;
}

export interface DemoSentencesResponse {
  description: string;
  sentences: DemoSentence[];
  demoScenarios: DemoScenario[];
}

export interface SearchEnhancements {
  useHybridSearch: boolean;
  useQueryExpansion: boolean;
  useReranking: boolean;
  useMultiVectorSearch: boolean;
  useContextualEmbeddings: boolean;
  useHyDE: boolean;
  useHnswApproximate: boolean;
  hnswEfSearch: number;
}

export interface VectorSearchDemoRequest {
  query: string;
  enhancements?: SearchEnhancements;
  topK?: number;
  minScore?: number;
}

export interface SearchResultItem {
  id: string;
  text: string;
  category: string;
  score: number;
  matchedKeywords: string[];
}

export interface SearchResultSet {
  results: SearchResultItem[];
  searchTimeMs: number;
  expandedQuery?: string;
  hypotheticalDocument?: string;
}

export interface VectorSearchDemoResponse {
  standardResults: SearchResultSet;
  enhancedResults?: SearchResultSet;
  originalQuery: string;
  appliedEnhancements: SearchEnhancements;
}

export interface InitializeDemoResponse {
  success: boolean;
  sentencesEmbedded: number;
  collectionName: string;
  message: string;
}

export interface DemoStatus {
  isInitialized: boolean;
  embeddedSentenceCount: number;
  embeddingModel: string | null;
}
