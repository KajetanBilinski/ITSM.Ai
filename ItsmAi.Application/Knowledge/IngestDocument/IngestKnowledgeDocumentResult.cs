namespace ItsmAi.Application.Knowledge.IngestDocument;

public record IngestKnowledgeDocumentResult(
    Guid DocumentId,
    int ChunkCount);