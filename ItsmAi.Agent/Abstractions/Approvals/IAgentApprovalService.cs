namespace ItsmAi.Agent.Abstractions.Approvals;

public interface IAgentApprovalService
{
    Task<AgentApproval?> GetAsync(
        Guid approvalId,
        CancellationToken cancellationToken = default);

    Task<AgentApprovalDecisionResult> ApproveAsync(
        Guid approvalId,
        CancellationToken cancellationToken = default);

    Task<AgentApprovalDecisionResult> RejectAsync(
        Guid approvalId,
        CancellationToken cancellationToken = default);
}