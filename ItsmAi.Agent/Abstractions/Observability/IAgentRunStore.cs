namespace ItsmAi.Agent.Abstractions.Observability;

public interface IAgentRunStore
{
    Task<Guid> StartAsync(
        string userMessage,
        CancellationToken cancellationToken = default);

    Task RecordToolExecutionAsync(
        Guid agentRunId,
        ToolExecutionRecord execution,
        CancellationToken cancellationToken = default);

    Task CompleteAsync(
        Guid agentRunId,
        string finalResponse,
        CancellationToken cancellationToken = default);

    Task FailAsync(
        Guid agentRunId,
        string error,
        CancellationToken cancellationToken = default);

    Task MarkPendingApprovalAsync(
        Guid agentRunId,
        CancellationToken cancellationToken = default);
}