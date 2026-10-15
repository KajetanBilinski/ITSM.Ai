using System.Text.Json;
using ItsmAi.Agent.Abstractions.Approvals;
using ItsmAi.Agent.Abstractions.Observability;
using ItsmAi.Agent.Abstractions.Tools;
using Microsoft.Extensions.Logging;

namespace ItsmAi.Agent.Approvals;

public class AgentApprovalService
    : IAgentApprovalService
{
    private readonly IAgentApprovalStore _approvalStore;
    private readonly IAgentToolRegistry _toolRegistry;
    private readonly IAgentRunStore _agentRunStore;
    private readonly ILogger<AgentApprovalService> _logger;

    public AgentApprovalService(
        IAgentApprovalStore approvalStore,
        IAgentToolRegistry toolRegistry,
        IAgentRunStore agentRunStore,
        ILogger<AgentApprovalService> logger)
    {
        _approvalStore = approvalStore;
        _toolRegistry = toolRegistry;
        _agentRunStore = agentRunStore;
        _logger = logger;
    }

    public Task<AgentApproval?> GetAsync(
        Guid approvalId,
        CancellationToken cancellationToken = default)
    {
        return _approvalStore.GetByIdAsync(
            approvalId,
            cancellationToken);
    }

    public async Task<AgentApprovalDecisionResult> ApproveAsync(
        Guid approvalId,
        CancellationToken cancellationToken = default)
    {
        var approval = await _approvalStore.GetByIdAsync(
            approvalId,
            cancellationToken);

        if (approval is null)
        {
            return new AgentApprovalDecisionResult(
                AgentApprovalDecisionStatus.NotFound,
                "Approval was not found.");
        }

        if (approval.Status != "Pending")
        {
            return new AgentApprovalDecisionResult(
                AgentApprovalDecisionStatus.NotPending,
                $"Approval is already in status '{approval.Status}'.");
        }

        var tool = _toolRegistry.Get(
            approval.ToolName);

        if (tool is null)
        {
            return new AgentApprovalDecisionResult(
                AgentApprovalDecisionStatus.Failed,
                $"Tool '{approval.ToolName}' is no longer registered.");
        }

        if (tool.Access != AgentToolAccess.Write)
        {
            return new AgentApprovalDecisionResult(
                AgentApprovalDecisionStatus.Failed,
                $"Tool '{tool.Name}' is not a write tool.");
        }

        var executionStarted =
            await _approvalStore.TryBeginExecutionAsync(
                approvalId,
                cancellationToken);

        if (!executionStarted)
        {
            return new AgentApprovalDecisionResult(
                AgentApprovalDecisionStatus.NotPending,
                "Approval is no longer pending.");
        }

        try
        {
            using var document =
                JsonDocument.Parse(
                    approval.ArgumentsJson);

            var arguments =
                document.RootElement.Clone();

            var startedAt =
                DateTime.UtcNow;

            _logger.LogInformation(
                "Executing approved tool {ToolName} for approval {ApprovalId}",
                tool.Name,
                approval.Id);

            var toolResult =
                await tool.ExecuteAsync(
                    arguments,
                    cancellationToken);

            var completedAt =
                DateTime.UtcNow;

            var serializedResult =
                JsonSerializer.Serialize(
                    toolResult);

            await _agentRunStore.RecordToolExecutionAsync(
                approval.AgentRunId,
                new ToolExecutionRecord(
                    ToolName: tool.Name,
                    ArgumentsJson: approval.ArgumentsJson,
                    ResultJson: serializedResult,
                    Success: toolResult.Success,
                    StartedAt: startedAt,
                    CompletedAt: completedAt),
                cancellationToken);

            if (!toolResult.Success)
            {
                await _approvalStore.MarkFailedAsync(
                    approvalId,
                    cancellationToken);

                var error =
                    toolResult.Error ??
                    "Approved tool execution failed.";

                await _agentRunStore.FailAsync(
                    approval.AgentRunId,
                    error,
                    cancellationToken);

                return new AgentApprovalDecisionResult(
                    AgentApprovalDecisionStatus.Failed,
                    error,
                    toolResult);
            }

            await _approvalStore.MarkApprovedAsync(
                approvalId,
                cancellationToken);

            var finalMessage =
                $"Approved tool '{tool.Name}' was executed successfully.";

            await _agentRunStore.CompleteAsync(
                approval.AgentRunId,
                finalMessage,
                cancellationToken);

            _logger.LogInformation(
                "Approval {ApprovalId} completed successfully using tool {ToolName}",
                approval.Id,
                tool.Name);

            return new AgentApprovalDecisionResult(
                AgentApprovalDecisionStatus.Approved,
                finalMessage,
                toolResult);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Execution of approval {ApprovalId} failed",
                approvalId);

            await _approvalStore.MarkFailedAsync(
                approvalId,
                CancellationToken.None);

            await _agentRunStore.FailAsync(
                approval.AgentRunId,
                exception.Message,
                CancellationToken.None);

            throw;
        }
    }

    public async Task<AgentApprovalDecisionResult> RejectAsync(
        Guid approvalId,
        CancellationToken cancellationToken = default)
    {
        var approval = await _approvalStore.GetByIdAsync(
            approvalId,
            cancellationToken);

        if (approval is null)
        {
            return new AgentApprovalDecisionResult(
                AgentApprovalDecisionStatus.NotFound,
                "Approval was not found.");
        }

        if (approval.Status != "Pending")
        {
            return new AgentApprovalDecisionResult(
                AgentApprovalDecisionStatus.NotPending,
                $"Approval is already in status '{approval.Status}'.");
        }

        var rejected =
            await _approvalStore.TryRejectAsync(
                approvalId,
                cancellationToken);

        if (!rejected)
        {
            return new AgentApprovalDecisionResult(
                AgentApprovalDecisionStatus.NotPending,
                "Approval is no longer pending.");
        }

        var finalMessage =
            $"Approval for tool '{approval.ToolName}' was rejected.";

        await _agentRunStore.CompleteAsync(
            approval.AgentRunId,
            finalMessage,
            cancellationToken);

        _logger.LogInformation(
            "Approval {ApprovalId} for tool {ToolName} was rejected",
            approval.Id,
            approval.ToolName);

        return new AgentApprovalDecisionResult(
            AgentApprovalDecisionStatus.Rejected,
            finalMessage);
    }
}