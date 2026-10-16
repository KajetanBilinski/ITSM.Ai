namespace ItsmAi.Application.Knowledge.Search;

public record SearchKnowledgeQuery(
    string Query,
    int Limit = 5);