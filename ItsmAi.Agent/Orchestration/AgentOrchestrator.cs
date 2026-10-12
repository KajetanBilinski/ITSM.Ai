using System.Text.Json;
using ItsmAi.Agent.Abstractions.Agent;
using ItsmAi.Agent.Abstractions.Llm;
using ItsmAi.Agent.Abstractions.Tools;

namespace ItsmAi.Agent.Orchestration;

public class AgentOrchestrator : IAgentOrchestrator
{
    private readonly ILlmProvider _llmProvider;
    private readonly IAgentToolRegistry _toolRegistry;

    public AgentOrchestrator(
        ILlmProvider llmProvider,
        IAgentToolRegistry toolRegistry)
    {
        _llmProvider = llmProvider;
        _toolRegistry = toolRegistry;
    }

    public async Task<AgentResponse> RunAsync(
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<LlmMessage>
        {
            new(
                LlmMessageRole.User,
                userMessage)
        };

        var tools = _toolRegistry.Tools
            .Select(tool => new LlmToolDefinition(
                tool.Name,
                tool.Description,
                tool.ParametersJsonSchema))
            .ToList();

        var firstResponse = await _llmProvider.CompleteAsync(
            new LlmRequest(
                messages,
                tools),
            cancellationToken);

        if (firstResponse.ToolCalls.Count == 0)
        {
            return new AgentResponse(
                firstResponse.Content);
        }

        if (firstResponse.ToolCalls.Count > 1)
        {
            throw new InvalidOperationException(
                "Parallel tool calls are not supported yet.");
        }

        var toolCall = firstResponse.ToolCalls[0];

        messages.Add(
            new LlmMessage(
                LlmMessageRole.Assistant,
                firstResponse.Content,
                firstResponse.ToolCalls));

        var tool = _toolRegistry.Get(
            toolCall.Name);

        if (tool is null)
        {
            throw new InvalidOperationException(
                $"LLM requested unknown tool '{toolCall.Name}'.");
        }

        var toolResult = await tool.ExecuteAsync(
            toolCall.Arguments,
            cancellationToken);

        var serializedToolResult =
            JsonSerializer.Serialize(toolResult);

        messages.Add(
            new LlmMessage(
                LlmMessageRole.Tool,
                serializedToolResult,
                ToolName: toolCall.Name,
                ToolCallId: toolCall.Id));

        var finalResponse = await _llmProvider.CompleteAsync(
            new LlmRequest(
                messages,
                tools),
            cancellationToken);

        if (finalResponse.ToolCalls.Count > 0)
        {
            throw new InvalidOperationException(
                "Additional tool calls are not supported yet.");
        }

        return new AgentResponse(
            finalResponse.Content);
    }
}