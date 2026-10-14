namespace ItsmAi.Agent.Abstractions.Observability;

public record ToolExecutionRecord(
    string ToolName,
    string ArgumentsJson,
    string ResultJson,
    bool Success,
    DateTime StartedAt,
    DateTime CompletedAt);