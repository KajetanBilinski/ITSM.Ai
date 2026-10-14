using System.Text.Json;

namespace ItsmAi.Agent.Abstractions.Agent;

public record AgentApprovalRequest(
    Guid ApprovalId,
    string ToolName,
    JsonElement Arguments);