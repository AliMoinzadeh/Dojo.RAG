import type { RetrievedChunk } from '../types/api';

interface Props {
  chunks: RetrievedChunk[];
}

export function RelevanceScoreDisplay({ chunks }: Props) {
  const maxScore = Math.max(...chunks.map(c => c.relevanceScore), 1);

  return (
    <div className="bg-white p-4 rounded-lg border">
      <h4 className="text-sm font-semibold text-gray-700 mb-3">Retrieved Chunks</h4>
      
      <div className="space-y-3">
        {chunks.map((chunk, index) => (
          <div key={chunk.id} className="border rounded-lg p-3 bg-gray-50">
            <div className="flex justify-between items-center mb-2">
              <span className="text-sm font-medium text-gray-700">
                #{index + 1} - {chunk.sourceFileName}
              </span>
              <div className="flex items-center gap-2">
                <div className="w-24 h-2 bg-gray-200 rounded-full overflow-hidden">
                  <div
                    className="h-full bg-green-500 transition-all"
                    style={{ width: `${(chunk.relevanceScore / maxScore) * 100}%` }}
                  />
                </div>
                <span className="text-xs font-mono text-gray-600 w-16 text-right">
                  {chunk.relevanceScore.toFixed(4)}
                </span>
              </div>
            </div>
            
            <p className="text-sm text-gray-600 line-clamp-3">
              {chunk.content}
            </p>
            
            <div className="mt-2 text-xs text-gray-400">
              Chunk #{chunk.chunkIndex}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
