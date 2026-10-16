using ItsmAi.Domain.Entities;
using ItsmAi.Infrastructure.Ai.Approvals;
using ItsmAi.Infrastructure.AI.Observability;
using ItsmAi.Infrastructure.Knowledge.Persistence;
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
    public DbSet<KnowledgeDocumentEntity> KnowledgeDocuments => Set<KnowledgeDocumentEntity>();
    public DbSet<KnowledgeChunkEntity> KnowledgeChunks => Set<KnowledgeChunkEntity>();
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}