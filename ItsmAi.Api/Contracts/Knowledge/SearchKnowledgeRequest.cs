namespace ItsmAi.Api.Contracts.Knowledge;

public record SearchKnowledgeRequest(
    string Query,
    int Limit = 5);