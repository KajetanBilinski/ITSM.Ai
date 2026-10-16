using ItsmAi.Application.Knowledge.Persistence;

namespace ItsmAi.Infrastructure.Knowledge.Persistence;

public class KnowledgeDocumentEntity
{
    public Guid Id { get; set; }

    public string Title { get; set; } =
        string.Empty;

    public string Content { get; set; } =
        string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<KnowledgeChunkEntity> Chunks { get; set; } =
        [];
}