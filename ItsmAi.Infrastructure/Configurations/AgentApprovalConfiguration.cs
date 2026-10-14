using ItsmAi.Infrastructure.Ai.Approvals;
using ItsmAi.Infrastructure.AI.Observability;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItsmAi.Infrastructure.Persistence.Configurations;

public class AgentApprovalConfiguration
    : IEntityTypeConfiguration<AgentApprovalEntity>
{
    public void Configure(
        EntityTypeBuilder<AgentApprovalEntity> builder)
    {
        builder.ToTable("AgentApprovals");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ToolName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ArgumentsJson)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.AgentRunId);

        builder.HasOne<AgentRunEntity>()
            .WithMany()
            .HasForeignKey(x => x.AgentRunId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}