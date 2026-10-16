using ItsmAi.Infrastructure.Knowledge.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItsmAi.Infrastructure.Persistence.Configurations;

public class KnowledgeDocumentConfiguration
    : IEntityTypeConfiguration<KnowledgeDocumentEntity>
{
    public void Configure(
        EntityTypeBuilder<KnowledgeDocumentEntity> builder)
    {
        builder.ToTable("KnowledgeDocuments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasMany(x => x.Chunks)
            .WithOne(x => x.Document)
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}