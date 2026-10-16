namespace ItsmAi.Application.Abstractions.Knowledge;

public interface IKnowledgeRepository
{
    Task AddAsync(
        KnowledgeDocumentData document,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<KnowledgeSearchResult>> SearchAsync(
        float[] queryEmbedding,
        int limit,
        CancellationToken cancellationToken = default);
}