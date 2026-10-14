using ItsmAi.Domain.Entities;
using ItsmAi.Infrastructure.Ai.Approvals;
using ItsmAi.Infrastructure.AI.Observability;
using Microsoft.EntityFrameworkCore;

namespace ItsmAi.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<IncidentComment> IncidentComments => Set<IncidentComment>();
    public DbSet<Requester> Requesters => Set<Requester>();
    public DbSet<AgentRunEntity> AgentRuns => Set<AgentRunEntity>();
    public DbSet<ToolExecutionEntity> ToolExecutions => Set<ToolExecutionEntity>();
    public DbSet<AgentApprovalEntity> AgentApprovals => Set<AgentApprovalEntity>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}