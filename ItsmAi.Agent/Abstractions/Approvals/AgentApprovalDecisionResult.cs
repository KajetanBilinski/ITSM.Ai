using ItsmAi.Agent.Abstractions.Tools;

namespace ItsmAi.Agent.Abstractions.Approvals;

public record AgentApprovalDecisionResult(
    AgentApprovalDecisionStatus Status,
    string Message,
    AgentToolResult? ToolResult = null);