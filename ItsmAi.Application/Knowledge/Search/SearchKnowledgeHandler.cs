using ItsmAi.Application.Abstractions.Ai;
using ItsmAi.Application.Abstractions.Knowledge;

namespace ItsmAi.Application.Knowledge.Search;

public class SearchKnowledgeHandler
{
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly IKnowledgeRepository _knowledgeRepository;

    public SearchKnowledgeHandler(
        IEmbeddingProvider embeddingProvider,
        IKnowledgeRepository knowledgeRepository)
    {
        _embeddingProvider = embeddingProvider;
        _knowledgeRepository = knowledgeRepository;
    }

    public async Task<IReadOnlyList<KnowledgeSearchResult>> HandleAsync(
        SearchKnowledgeQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Query))
        {
            throw new ArgumentException(
                "Search query is required.");
        }

        var limit = Math.Clamp(
            query.Limit,
            1,
            10);

        var embedding =
            await _embeddingProvider.GenerateAsync(
                query.Query,
                cancellationToken);

        return await _knowledgeRepository.SearchAsync(
            embedding,
            limit,
            cancellationToken);
    }
}