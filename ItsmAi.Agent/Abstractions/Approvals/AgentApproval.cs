namespace ItsmAi.Agent.Abstractions.Approvals;

public record AgentApproval(
    Guid Id,
    Guid AgentRunId,
    string ToolName,
    string ArgumentsJson,
    string Status,
    DateTime CreatedAt,
    DateTime? DecidedAt);