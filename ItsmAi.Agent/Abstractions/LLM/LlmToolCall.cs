using System.Text.Json;

namespace ItsmAi.Agent.Abstractions.Llm;

public record LlmToolCall(
    string Name,
    JsonElement Arguments,
    string? Id = null);