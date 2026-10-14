namespace ItsmAi.Agent.Abstractions.Agent;

public record AgentResponse(
    string Content,
    AgentApprovalRequest? ApprovalRequired = null);