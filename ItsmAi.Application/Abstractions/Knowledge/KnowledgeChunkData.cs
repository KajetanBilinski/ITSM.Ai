namespace ItsmAi.Application.Abstractions.Knowledge;

public record KnowledgeChunkData(
    Guid Id,
    int ChunkIndex,
    string Content,
    float[] Embedding);