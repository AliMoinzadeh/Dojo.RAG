import { useState } from 'react'
import { ChatWindow } from './components/ChatWindow'
import { DocumentUpload } from './components/DocumentUpload'
import { ConfigDisplay } from './components/ConfigDisplay'
import { DocumentManager } from './components/DocumentManager'
import { VectorSearchDemo } from './components/VectorSearchDemo'

type TabType = 'rag' | 'vector-demo';

function App() {
  const [activeTab, setActiveTab] = useState<TabType>('rag');

  return (
    <div className="min-h-screen bg-gray-900">
      {/* Header */}
      <header className="bg-gradient-to-r from-blue-600 to-purple-600 text-white py-6 px-8">
        <div className="max-w-7xl mx-auto">
          <h1 className="text-3xl font-bold">🔍 RAG Demo</h1>
          <p className="text-blue-100 mt-1">
            Retrieval-Augmented Generation Educational Demo
          </p>
        </div>
      </header>

      {/* Tab Navigation */}
      <div className="bg-gray-800 border-b border-gray-700">
        <div className="max-w-7xl mx-auto px-8">
          <nav className="flex gap-1">
            <button
              onClick={() => setActiveTab('rag')}
              className={`px-6 py-3 text-sm font-medium transition-colors ${
                activeTab === 'rag'
                  ? 'text-white bg-gray-900 border-b-2 border-blue-500'
                  : 'text-gray-400 hover:text-white hover:bg-gray-700'
              }`}
            >
              📚 RAG Chat
            </button>
            <button
              onClick={() => setActiveTab('vector-demo')}
              className={`px-6 py-3 text-sm font-medium transition-colors ${
                activeTab === 'vector-demo'
                  ? 'text-white bg-gray-900 border-b-2 border-blue-500'
                  : 'text-gray-400 hover:text-white hover:bg-gray-700'
              }`}
            >
              🎯 Vector Search Demo
            </button>
          </nav>
        </div>
      </div>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-8 py-8">
        {activeTab === 'rag' ? (
          <>
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
              {/* Left Column - Chat */}
              <div className="lg:col-span-2">
                <div className="h-[600px]">
                  <ChatWindow />
                </div>
              </div>

              {/* Right Column - Controls */}
              <div className="space-y-6">
                <ConfigDisplay />
                <DocumentUpload />
              </div>
            </div>

            {/* Document Manager Section */}
            <div className="mt-8">
              <DocumentManager />
            </div>

            {/* Educational Note */}
            <div className="mt-8 bg-yellow-900/30 border border-yellow-600/50 rounded-lg p-6">
              <h3 className="text-lg font-semibold text-yellow-400 mb-2">
                📚 How RAG Works
              </h3>
              <ol className="list-decimal list-inside space-y-2 text-yellow-200/80">
                <li><strong>Document Ingestion:</strong> Upload documents which get split into chunks and embedded as vectors</li>
                <li><strong>Query Embedding:</strong> Your question is converted to a vector using the same embedding model</li>
                <li><strong>Vector Search:</strong> Find the most similar document chunks using cosine similarity</li>
                <li><strong>Context Augmentation:</strong> Retrieved chunks are added to the prompt as context</li>
                <li><strong>LLM Generation:</strong> The language model generates an answer using the context</li>
              </ol>
              <p className="mt-4 text-sm text-yellow-500">
                Enable "Include debug info" in the chat to see the RAG pipeline in action!
              </p>
            </div>
          </>
        ) : (
          <VectorSearchDemo />
        )}
      </main>

      {/* Footer */}
      <footer className="bg-gray-800 text-gray-400 py-4 px-8 mt-8">
        <div className="max-w-7xl mx-auto text-center text-sm">
          RAG Demo - Built with .NET 9, React, and ❤️ for teaching AI concepts
        </div>
      </footer>
    </div>
  )
}

export default App
