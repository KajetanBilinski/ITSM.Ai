namespace ItsmAi.Api.Contracts.Knowledge;

public record CreateKnowledgeDocumentRequest(
    string Title,
    string Content);