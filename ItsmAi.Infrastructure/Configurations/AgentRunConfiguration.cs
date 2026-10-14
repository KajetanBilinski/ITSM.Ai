using ItsmAi.Infrastructure.AI.Observability;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItsmAi.Infrastructure.Configurations;

public class AgentRunConfiguration
    : IEntityTypeConfiguration<AgentRunEntity>
{
    public void Configure(
        EntityTypeBuilder<AgentRunEntity> builder)
    {
        builder.ToTable("AgentRuns");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserMessage)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FinalResponse)
            .HasMaxLength(8000);

        builder.Property(x => x.Error)
            .HasMaxLength(8000);

        builder.Property(x => x.StartedAt)
            .IsRequired();

        builder.HasMany(x => x.ToolExecutions)
            .WithOne(x => x.AgentRun)
            .HasForeignKey(x => x.AgentRunId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}