namespace ItsmAi.Agent.Tools.Knowledge;

public record SearchKnowledgeToolArguments(
    string Query,
    int Limit = 5);