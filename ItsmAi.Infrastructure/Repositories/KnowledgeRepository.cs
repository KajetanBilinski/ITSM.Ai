using ItsmAi.Application.Abstractions.Knowledge;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

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

    public async Task<IReadOnlyList<KnowledgeSearchResult>> SearchAsync(
    float[] queryEmbedding,
    int limit,
    CancellationToken cancellationToken = default)
    {
        var queryVector =
            new Vector(queryEmbedding);

        var results = await _dbContext.KnowledgeChunks
            .AsNoTracking()
            .Select(chunk => new
            {
                chunk.Id,
                chunk.DocumentId,
                DocumentTitle = chunk.Document.Title,
                chunk.Content,
                chunk.ChunkIndex,

                //less distance means more similar
                Distance =
                    chunk.Embedding.CosineDistance(
                        queryVector)
            })
            .OrderBy(x => x.Distance)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return results
            .Select(x =>
                new KnowledgeSearchResult(
                    ChunkId: x.Id,
                    DocumentId: x.DocumentId,
                    DocumentTitle: x.DocumentTitle,
                    Content: x.Content,
                    ChunkIndex: x.ChunkIndex,
                    Similarity: 1.0 - x.Distance))
            .ToList();
    }
}