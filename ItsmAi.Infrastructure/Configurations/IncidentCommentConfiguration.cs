using ItsmAi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItsmAi.Infrastructure.Configurations;

public class IncidentCommentConfiguration
    : IEntityTypeConfiguration<IncidentComment>
{
    public void Configure(
        EntityTypeBuilder<IncidentComment> builder)
    {
        builder.ToTable("IncidentComments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IncidentId)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.AuthorType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }
}