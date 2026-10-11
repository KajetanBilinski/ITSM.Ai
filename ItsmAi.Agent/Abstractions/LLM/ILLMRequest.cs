namespace ItsmAi.Agent.Abstractions.Llm;

public record LlmRequest(
    string UserMessage,
    IReadOnlyCollection<LlmToolDefinition> Tools,
    string? ToolResult = null);