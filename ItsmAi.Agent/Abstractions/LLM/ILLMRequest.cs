namespace ItsmAi.Agent.Abstractions.Llm;

public record LlmRequest(
    IReadOnlyList<LlmMessage> Messages,
    IReadOnlyCollection<LlmToolDefinition> Tools,
    bool Think = false);