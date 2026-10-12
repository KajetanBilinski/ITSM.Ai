namespace ItsmAi.Agent.Abstractions.Llm;

public record LlmRequest(
    string? UserMessage,
    IReadOnlyCollection<LlmToolDefinition> Tools,
    string? PreviousResponseId = null,
    LlmToolOutput? ToolOutput = null);