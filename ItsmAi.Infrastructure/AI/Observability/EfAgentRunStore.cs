using ItsmAi.Agent.Abstractions.Observability;
using Microsoft.EntityFrameworkCore;

namespace ItsmAi.Infrastructure.AI.Observability;

public class EfAgentRunStore : IAgentRunStore
{
    private readonly AppDbContext _dbContext;

    public EfAgentRunStore(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> StartAsync(
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        var run = new AgentRunEntity
        {
            Id = Guid.NewGuid(),
            UserMessage = userMessage,
            Status = "Running",
            StartedAt = DateTime.UtcNow
        };

        await _dbContext.AgentRuns.AddAsync(
            run,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return run.Id;
    }

    public async Task RecordToolExecutionAsync(
        Guid agentRunId,
        ToolExecutionRecord execution,
        CancellationToken cancellationToken = default)
    {
        var entity = new ToolExecutionEntity
        {
            Id = Guid.NewGuid(),
            AgentRunId = agentRunId,
            ToolName = execution.ToolName,
            ArgumentsJson = execution.ArgumentsJson,
            ResultJson = execution.ResultJson,
            Success = execution.Success,
            StartedAt = execution.StartedAt,
            CompletedAt = execution.CompletedAt
        };

        await _dbContext.ToolExecutions.AddAsync(
            entity,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task CompleteAsync(
        Guid agentRunId,
        string finalResponse,
        CancellationToken cancellationToken = default)
    {
        var run = await _dbContext.AgentRuns
            .SingleOrDefaultAsync(
                x => x.Id == agentRunId,
                cancellationToken);

        if (run is null)
        {
            throw new InvalidOperationException(
                $"Agent run '{agentRunId}' was not found.");
        }

        run.Status = "Completed";
        run.FinalResponse = finalResponse;
        run.CompletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task FailAsync(
        Guid agentRunId,
        string error,
        CancellationToken cancellationToken = default)
    {
        var run = await _dbContext.AgentRuns
            .SingleOrDefaultAsync(
                x => x.Id == agentRunId,
                cancellationToken);

        if (run is null)
        {
            return;
        }

        run.Status = "Failed";
        run.Error = error;
        run.CompletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task MarkPendingApprovalAsync(
        Guid agentRunId,
        CancellationToken cancellationToken = default)
    {
        var run = await _dbContext.AgentRuns
            .SingleOrDefaultAsync(
                x => x.Id == agentRunId,
                cancellationToken);

        if (run is null)
        {
            throw new InvalidOperationException(
                $"Agent run '{agentRunId}' was not found.");
        }

        run.Status = "PendingApproval";

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}