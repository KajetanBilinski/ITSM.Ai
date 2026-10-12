namespace ItsmAi.Agent.Abstractions.Llm;

public record LlmResponse(
    string Content,
    IReadOnlyList<LlmToolCall> ToolCalls);