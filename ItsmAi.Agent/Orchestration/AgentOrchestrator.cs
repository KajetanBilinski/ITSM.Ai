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
        var tools = _toolRegistry.Tools
            .Select(tool => new LlmToolDefinition(
                tool.Name,
                tool.Description,
                tool.ParametersJsonSchema))
            .ToList();

        var firstResponse = await _llmProvider.CompleteAsync(
            new LlmRequest(
                userMessage,
                tools),
            cancellationToken);

        if (firstResponse is LlmTextResponse textResponse)
        {
            return new AgentResponse(
                textResponse.Content);
        }

        if (firstResponse is not LlmToolCallResponse toolCall)
        {
            throw new InvalidOperationException(
                "Unsupported LLM response.");
        }

        var tool = _toolRegistry.Get(
            toolCall.ToolName);

        if (tool is null)
        {
            throw new InvalidOperationException(
                $"LLM requested unknown tool '{toolCall.ToolName}'.");
        }

        var toolResult = await tool.ExecuteAsync(
            toolCall.Arguments,
            cancellationToken);

        var serializedToolResult =
            JsonSerializer.Serialize(toolResult);

        var finalResponse = await _llmProvider.CompleteAsync(
            new LlmRequest(
                userMessage,
                tools,
                serializedToolResult),
            cancellationToken);

        if (finalResponse is not LlmTextResponse finalTextResponse)
        {
            throw new InvalidOperationException(
                "LLM did not return a final text response.");
        }

        return new AgentResponse(
            finalTextResponse.Content);
    }
}