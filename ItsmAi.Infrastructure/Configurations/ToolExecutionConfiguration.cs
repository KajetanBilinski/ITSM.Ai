using ItsmAi.Infrastructure.AI.Observability;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItsmAi.Infrastructure.Configurations;

public class ToolExecutionConfiguration
    : IEntityTypeConfiguration<ToolExecutionEntity>
{
    public void Configure(
        EntityTypeBuilder<ToolExecutionEntity> builder)
    {
        builder.ToTable("ToolExecutions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ToolName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ArgumentsJson)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.ResultJson)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.StartedAt)
            .IsRequired();

        builder.Property(x => x.CompletedAt)
            .IsRequired();

        builder.HasIndex(x => x.AgentRunId);
    }
}