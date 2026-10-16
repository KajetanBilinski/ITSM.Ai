using ItsmAi.Application.Abstractions.Knowledge;
using ItsmAi.Infrastructure.Persistence;
using Pgvector;

namespace ItsmAi.Infrastructure.Knowledge.Persistence;

public class KnowledgeRepository
    : IKnowledgeRepository
{
    private readonly AppDbContext _dbContext;

    public KnowledgeRepository(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        KnowledgeDocumentData document,
        CancellationToken cancellationToken = default)
    {
        var entity =
            new KnowledgeDocumentEntity
            {
                Id = document.Id,
                Title = document.Title,
                Content = document.Content,
                CreatedAt = document.CreatedAt,
                Chunks = document.Chunks
                    .Select(chunk =>
                        new KnowledgeChunkEntity
                        {
                            Id = chunk.Id,
                            DocumentId = document.Id,
                            ChunkIndex = chunk.ChunkIndex,
                            Content = chunk.Content,

                            Embedding =
                                new Vector(
                                    chunk.Embedding)
                        })
                    .ToList()
            };

        await _dbContext.KnowledgeDocuments.AddAsync(
            entity,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}