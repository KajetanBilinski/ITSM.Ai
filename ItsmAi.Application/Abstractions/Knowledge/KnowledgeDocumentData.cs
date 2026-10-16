namespace ItsmAi.Application.Abstractions.Knowledge;

public record KnowledgeDocumentData(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    IReadOnlyCollection<KnowledgeChunkData> Chunks);