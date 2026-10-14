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
}