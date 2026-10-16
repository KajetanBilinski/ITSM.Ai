namespace ItsmAi.Application.Abstractions.Knowledge;

public record KnowledgeSearchResult(
    Guid ChunkId,
    Guid DocumentId,
    string DocumentTitle,
    string Content,
    int ChunkIndex,
    double Similarity);