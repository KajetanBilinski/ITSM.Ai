namespace ItsmAi.Agent.Abstractions.Llm;

public record LlmMessage(
    LlmMessageRole Role,
    string Content,
    IReadOnlyList<LlmToolCall>? ToolCalls = null,
    string? ToolName = null,
    string? ToolCallId = null);