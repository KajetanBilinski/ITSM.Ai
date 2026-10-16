using ItsmAi.Application.Abstractions.Ai;
using ItsmAi.Application.Abstractions.Knowledge;

namespace ItsmAi.Application.Knowledge.Search;

public class SearchKnowledgeHandler
{
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly RagSearchOptions _options;

    public SearchKnowledgeHandler(
        IEmbeddingProvider embeddingProvider,
        IKnowledgeRepository knowledgeRepository,
        RagSearchOptions options)
    {
        _embeddingProvider = embeddingProvider;
        _knowledgeRepository = knowledgeRepository;
        _options = options;
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

        var limit = Math.Clamp(query.Limit, 1, 10);

        var embedding =
            await _embeddingProvider.GenerateAsync(
                query.Query,
                cancellationToken);

        var results =
            await _knowledgeRepository.SearchAsync(
                embedding,
                limit,
                cancellationToken);

        return results
            .Where(result =>
                result.Similarity >= _options.MinimumSimilarity)
            .ToList();
    }
}