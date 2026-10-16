namespace ItsmAi.Application.Abstractions.Knowledge;

public interface IKnowledgeRepository
{
    Task AddAsync(
        KnowledgeDocumentData document,
        CancellationToken cancellationToken = default);
}