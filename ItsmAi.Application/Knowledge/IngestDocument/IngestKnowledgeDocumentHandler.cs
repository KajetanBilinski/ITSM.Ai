using ItsmAi.Application.Abstractions.Ai;
using ItsmAi.Application.Abstractions.Knowledge;

namespace ItsmAi.Application.Knowledge.IngestDocument;

public class IngestKnowledgeDocumentHandler
{
    private readonly ITextChunker _textChunker;
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly IKnowledgeRepository _knowledgeRepository;

    public IngestKnowledgeDocumentHandler(
        ITextChunker textChunker,
        IEmbeddingProvider embeddingProvider,
        IKnowledgeRepository knowledgeRepository)
    {
        _textChunker = textChunker;
        _embeddingProvider = embeddingProvider;
        _knowledgeRepository = knowledgeRepository;
    }

    public async Task<IngestKnowledgeDocumentResult> HandleAsync(
        IngestKnowledgeDocumentCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
        {
            throw new ArgumentException(
                "Knowledge document title is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Content))
        {
            throw new ArgumentException(
                "Knowledge document content is required.");
        }

        var chunks =
            _textChunker.Chunk(
                command.Content);

        if (chunks.Count == 0)
        {
            throw new InvalidOperationException(
                "Knowledge document did not produce any chunks.");
        }

        var embeddings =
            await _embeddingProvider.GenerateBatchAsync(
                chunks,
                cancellationToken);

        if (embeddings.Count != chunks.Count)
        {
            throw new InvalidOperationException(
                "Embedding count does not match chunk count.");
        }

        var documentId =
            Guid.NewGuid();

        var chunkData = new List<KnowledgeChunkData>();

        for (var index = 0;
             index < chunks.Count;
             index++)
        {
            chunkData.Add(
                new KnowledgeChunkData(
                    Id: Guid.NewGuid(),
                    ChunkIndex: index,
                    Content: chunks[index],
                    Embedding: embeddings[index]));
        }

        var document =
            new KnowledgeDocumentData(
                Id: documentId,
                Title: command.Title.Trim(),
                Content: command.Content.Trim(),
                CreatedAt: DateTime.UtcNow,
                Chunks: chunkData);

        await _knowledgeRepository.AddAsync(
            document,
            cancellationToken);

        return new IngestKnowledgeDocumentResult(
            DocumentId: documentId,
            ChunkCount: chunks.Count);
    }
}