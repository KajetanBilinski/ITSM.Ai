namespace ItsmAi.Agent.Abstractions.Llm;

public record LlmToolDefinition(
    string Name,
    string Description,
    string ParametersJsonSchema);