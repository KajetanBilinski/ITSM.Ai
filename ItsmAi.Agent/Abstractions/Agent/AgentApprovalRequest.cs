using System.Text.Json;

namespace ItsmAi.Agent.Abstractions.Agent;

public record AgentApprovalRequest(
    string ToolName,
    JsonElement Arguments);