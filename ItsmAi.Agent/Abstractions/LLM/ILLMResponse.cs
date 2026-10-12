using System.Text.Json;

namespace ItsmAi.Agent.Abstractions.Llm;

public abstract record LlmResponse;

public record LlmTextResponse(
    string Content,
    string ResponseId)
    : LlmResponse;

public record LlmToolCallResponse(
    string ToolName,
    JsonElement Arguments,
    string CallId,
    string ResponseId)
    : LlmResponse;