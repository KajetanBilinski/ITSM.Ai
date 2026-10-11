namespace ItsmAi.Agent.Abstractions.Tools;

public record AgentToolResult(
    bool Success,
    object? Data = null,
    string? Error = null);