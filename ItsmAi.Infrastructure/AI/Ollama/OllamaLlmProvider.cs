using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ItsmAi.Agent.Abstractions.Llm;
using Microsoft.Extensions.Configuration;

namespace ItsmAi.Infrastructure.Ai.Ollama;

public class OllamaLlmProvider : ILlmProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _model;

    public OllamaLlmProvider(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _model = configuration["Ollama:Model"]
            ?? throw new InvalidOperationException(
                "Ollama model is not configured.");
    }

    public async Task<LlmResponse> CompleteAsync(
        LlmRequest request,
        CancellationToken cancellationToken = default)
    {
        var messages = request.Messages
            .Select(MapMessage)
            .ToList();

        var tools = request.Tools
            .Select(MapTool)
            .ToList();

        var ollamaRequest = new OllamaChatRequest(
            Model: _model,
            Messages: messages,
            Tools: tools,
            Stream: false,
            Think: request.Think);

        using var response = await _httpClient.PostAsJsonAsync(
            "/api/chat",
            ollamaRequest,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var ollamaResponse =
            await response.Content.ReadFromJsonAsync<OllamaChatResponse>(
                cancellationToken: cancellationToken);

        if (ollamaResponse?.Message is null)
        {
            throw new InvalidOperationException(
                "Ollama returned an invalid response.");
        }

        var toolCalls = ollamaResponse.Message.ToolCalls?
            .Select(MapToolCall)
            .ToList()
            ?? [];

        return new LlmResponse(
            ollamaResponse.Message.Content ?? string.Empty,
            toolCalls);
    }

    private static OllamaMessage MapMessage(
        LlmMessage message)
    {
        var role = message.Role switch
        {
            LlmMessageRole.System => "system",
            LlmMessageRole.User => "user",
            LlmMessageRole.Assistant => "assistant",
            LlmMessageRole.Tool => "tool",

            _ => throw new ArgumentOutOfRangeException(
                nameof(message.Role),
                message.Role,
                "Unsupported LLM message role.")
        };

        List<OllamaToolCall>? toolCalls = null;

        if (message.ToolCalls is not null)
        {
            toolCalls = message.ToolCalls
                .Select(toolCall => new OllamaToolCall(
                    toolCall.Id,
                    new OllamaToolCallFunction(
                        toolCall.Name,
                        toolCall.Arguments)))
                .ToList();
        }

        return new OllamaMessage(
            Role: role,
            Content: message.Content,
            ToolCalls: toolCalls,
            ToolName: message.ToolName,
            ToolCallId: message.ToolCallId);
    }

    private static OllamaTool MapTool(
        LlmToolDefinition tool)
    {
        using var document =
            JsonDocument.Parse(
                tool.ParametersJsonSchema);

        var parameters =
            document.RootElement.Clone();

        return new OllamaTool(
            Type: "function",
            Function: new OllamaToolFunction(
                Name: tool.Name,
                Description: tool.Description,
                Parameters: parameters));
    }

    private static LlmToolCall MapToolCall(
        OllamaToolCall toolCall)
    {
        return new LlmToolCall(
            Name: toolCall.Function.Name,
            Arguments: toolCall.Function.Arguments,
            Id: toolCall.Id);
    }

    private sealed record OllamaChatRequest(
        [property: JsonPropertyName("model")]
        string Model,

        [property: JsonPropertyName("messages")]
        IReadOnlyList<OllamaMessage> Messages,

        [property: JsonPropertyName("tools")]
        IReadOnlyList<OllamaTool> Tools,

        [property: JsonPropertyName("stream")]
        bool Stream,

        [property: JsonPropertyName("think")]
        bool Think);

    private sealed record OllamaChatResponse(
        [property: JsonPropertyName("message")]
        OllamaMessage? Message);

    private sealed record OllamaMessage(
        [property: JsonPropertyName("role")]
        string Role,

        [property: JsonPropertyName("content")]
        string Content,

        [property: JsonPropertyName("tool_calls")]
        IReadOnlyList<OllamaToolCall>? ToolCalls = null,

        [property: JsonPropertyName("tool_name")]
        string? ToolName = null,

        [property: JsonPropertyName("tool_call_id")]
        string? ToolCallId = null);

    private sealed record OllamaTool(
        [property: JsonPropertyName("type")]
        string Type,

        [property: JsonPropertyName("function")]
        OllamaToolFunction Function);

    private sealed record OllamaToolFunction(
        [property: JsonPropertyName("name")]
        string Name,

        [property: JsonPropertyName("description")]
        string Description,

        [property: JsonPropertyName("parameters")]
        JsonElement Parameters);

    private sealed record OllamaToolCall(
        [property: JsonPropertyName("id")]
        string? Id,

        [property: JsonPropertyName("function")]
        OllamaToolCallFunction Function);

    private sealed record OllamaToolCallFunction(
        [property: JsonPropertyName("name")]
        string Name,

        [property: JsonPropertyName("arguments")]
        JsonElement Arguments);
}