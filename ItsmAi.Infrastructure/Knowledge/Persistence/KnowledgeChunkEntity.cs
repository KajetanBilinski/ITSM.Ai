using Pgvector;

namespace ItsmAi.Infrastructure.Knowledge.Persistence;

public class KnowledgeChunkEntity
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public int ChunkIndex { get; set; }

    public string Content { get; set; } =
        string.Empty;

    public Vector Embedding { get; set; } =
        null!;

    public KnowledgeDocumentEntity Document { get; set; } =
        null!;
}