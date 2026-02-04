import type { TokenUsage } from '../types/api';

interface Props {
  usage: TokenUsage;
}

export function ContextWindowGauge({ usage }: Props) {
  const segments = [
    { label: 'System', tokens: usage.systemPromptTokens, color: 'bg-purple-500' },
    { label: 'Context', tokens: usage.contextTokens, color: 'bg-blue-500' },
    { label: 'Query', tokens: usage.queryTokens, color: 'bg-green-500' },
    { label: 'Available', tokens: usage.maxContextTokens - usage.totalInputTokens, color: 'bg-gray-200' },
  ];

  return (
    <div className="bg-white p-4 rounded-lg border">
      <h4 className="text-sm font-semibold text-gray-700 mb-2">Context Window Usage</h4>
      
      {/* Progress bar */}
      <div className="flex h-6 rounded-lg overflow-hidden mb-2">
        {segments.map((seg) => {
          const percentage = (seg.tokens / usage.maxContextTokens) * 100;
          if (percentage < 0.5) return null;
          
          return (
            <div
              key={seg.label}
              className={`${seg.color} flex items-center justify-center text-xs text-white font-medium`}
              style={{ width: `${percentage}%` }}
              title={`${seg.label}: ${seg.tokens.toLocaleString()} tokens`}
            >
              {percentage > 8 && seg.label}
            </div>
          );
        })}
      </div>

      {/* Legend */}
      <div className="flex flex-wrap gap-4 text-xs">
        {segments.map((seg) => (
          <div key={seg.label} className="flex items-center gap-1">
            <div className={`w-3 h-3 rounded ${seg.color}`} />
            <span className="text-gray-600">
              {seg.label}: {seg.tokens.toLocaleString()}
            </span>
          </div>
        ))}
      </div>

      {/* Summary */}
      <div className="mt-2 text-sm text-gray-600">
        <strong>{usage.totalInputTokens.toLocaleString()}</strong> / {usage.maxContextTokens.toLocaleString()} tokens used
        ({usage.usagePercentage}%)
      </div>
    </div>
  );
}
