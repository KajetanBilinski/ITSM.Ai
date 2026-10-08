using ItsmAi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItsmAi.Infrastructure.Configurations;

public class RequesterConfiguration
    : IEntityTypeConfiguration<Requester>
{
    public void Configure(
        EntityTypeBuilder<Requester> builder)
    {
        builder.ToTable("Requesters");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();
    }
}