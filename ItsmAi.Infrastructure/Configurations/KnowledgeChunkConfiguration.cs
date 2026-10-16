using ItsmAi.Infrastructure.Knowledge.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ItsmAi.Infrastructure.Persistence.Configurations;

public class KnowledgeChunkConfiguration
    : IEntityTypeConfiguration<KnowledgeChunkEntity>
{
    public void Configure(
        EntityTypeBuilder<KnowledgeChunkEntity> builder)
    {
        builder.ToTable("KnowledgeChunks");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChunkIndex)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.Embedding)
            .HasColumnType("vector(768)")
            .IsRequired();

        builder.HasIndex(
            x => new
            {
                x.DocumentId,
                x.ChunkIndex
            })
            .IsUnique();
    }
}