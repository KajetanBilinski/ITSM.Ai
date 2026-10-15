using ItsmAi.Agent.Abstractions.Approvals;
using ItsmAi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ItsmAi.Infrastructure.Ai.Approvals;

public class EfAgentApprovalStore
    : IAgentApprovalStore
{
    private readonly AppDbContext _dbContext;

    public EfAgentApprovalStore(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AgentApproval> CreateAsync(
        Guid agentRunId,
        string toolName,
        string argumentsJson,
        CancellationToken cancellationToken = default)
    {
        var entity = new AgentApprovalEntity
        {
            Id = Guid.NewGuid(),
            AgentRunId = agentRunId,
            ToolName = toolName,
            ArgumentsJson = argumentsJson,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.AgentApprovals.AddAsync(
            entity,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return Map(entity);
    }

    public async Task<AgentApproval?> GetByIdAsync(
        Guid approvalId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.AgentApprovals
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Id == approvalId,
                cancellationToken);

        return entity is null
            ? null
            : Map(entity);
    }

    private static AgentApproval Map(
        AgentApprovalEntity entity)
    {
        return new AgentApproval(
            Id: entity.Id,
            AgentRunId: entity.AgentRunId,
            ToolName: entity.ToolName,
            ArgumentsJson: entity.ArgumentsJson,
            Status: entity.Status,
            CreatedAt: entity.CreatedAt,
            DecidedAt: entity.DecidedAt);
    }

    public async Task<bool> TryBeginExecutionAsync(
    Guid approvalId,
    CancellationToken cancellationToken = default)
    {
        var affectedRows = await _dbContext.AgentApprovals
            .Where(x =>
                x.Id == approvalId &&
                x.Status == "Pending")
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.Status,
                        "Executing"),
                cancellationToken);

        return affectedRows == 1;
    }

    public async Task MarkApprovedAsync(
    Guid approvalId,
    CancellationToken cancellationToken = default)
    {
        var decidedAt = DateTime.UtcNow;

        var affectedRows = await _dbContext.AgentApprovals
            .Where(x =>
                x.Id == approvalId &&
                x.Status == "Executing")
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.Status,
                        "Approved")
                    .SetProperty(
                        x => x.DecidedAt,
                        decidedAt),
                cancellationToken);

        if (affectedRows != 1)
        {
            throw new InvalidOperationException(
                $"Approval '{approvalId}' could not be marked as approved.");
        }
    }

    public async Task MarkFailedAsync(
    Guid approvalId,
    CancellationToken cancellationToken = default)
    {
        var decidedAt = DateTime.UtcNow;

        await _dbContext.AgentApprovals
            .Where(x =>
                x.Id == approvalId &&
                x.Status == "Executing")
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.Status,
                        "Failed")
                    .SetProperty(
                        x => x.DecidedAt,
                        decidedAt),
                cancellationToken);
    }

    public async Task<bool> TryRejectAsync(
    Guid approvalId,
    CancellationToken cancellationToken = default)
    {
        var decidedAt = DateTime.UtcNow;

        var affectedRows = await _dbContext.AgentApprovals
            .Where(x =>
                x.Id == approvalId &&
                x.Status == "Pending")
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.Status,
                        "Rejected")
                    .SetProperty(
                        x => x.DecidedAt,
                        decidedAt),
                cancellationToken);

        return affectedRows == 1;
    }
}