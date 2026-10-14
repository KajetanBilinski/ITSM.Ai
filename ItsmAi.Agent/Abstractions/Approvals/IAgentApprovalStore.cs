namespace ItsmAi.Agent.Abstractions.Approvals;

public interface IAgentApprovalStore
{
    Task<AgentApproval> CreateAsync(
        Guid agentRunId,
        string toolName,
        string argumentsJson,
        CancellationToken cancellationToken = default);

    Task<AgentApproval?> GetByIdAsync(
        Guid approvalId,
        CancellationToken cancellationToken = default);
}