using System.Text.Json;
using ItsmAi.Agent.Abstractions.Agent;
using ItsmAi.Agent.Abstractions.Llm;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Agent.Orchestration;

namespace ItsmAi.Tests.Agent;

public class AgentOrchestratorTests
{
    [Fact]
    public async Task RunAsync_WhenLlmReturnsText_ShouldReturnText()
    {
        var llm = new TextFakeLlmProvider(
            "Hello from agent.");

        var registry = new EmptyToolRegistry();

        var orchestrator = new AgentOrchestrator(
            llm,
            registry);

        var result = await orchestrator.RunAsync("Hello", TestContext.Current.CancellationToken);

        Assert.Equal(
            "Hello from agent.",
            result.Content);
    }

    private sealed class TextFakeLlmProvider : ILlmProvider
    {
        private readonly string _response;

        public TextFakeLlmProvider(string response)
        {
            _response = response;
        }

        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<LlmResponse>(
                new LlmTextResponse(_response, "resp_test_1"));
        }
    }

    private sealed class EmptyToolRegistry
        : IAgentToolRegistry
    {
        public IReadOnlyCollection<IAgentTool> Tools =>
            Array.Empty<IAgentTool>();

        public IAgentTool? Get(string name) => null;
    }

    private sealed class FakeTool : IAgentTool
    {
        public string Name => "test_tool";

        public string Description =>
            "Test tool.";

        public string ParametersJsonSchema =>
            """
        {
          "type": "object"
        }
        """;

        public Task<AgentToolResult> ExecuteAsync(
            JsonElement arguments,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new AgentToolResult(
                    true,
                    new
                    {
                        value = 42
                    }));
        }
    }

    private sealed class FakeToolRegistry
    : IAgentToolRegistry
    {
        private readonly IAgentTool _tool =
            new FakeTool();

        public IReadOnlyCollection<IAgentTool> Tools =>
            new[]
            {
            _tool
            };

        public IAgentTool? Get(string name)
        {
            return name == _tool.Name
                ? _tool
                : null;
        }
    }

    private sealed class ToolCallingFakeLlmProvider
    : ILlmProvider
    {
        private int _callCount;

        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            _callCount++;

            if (_callCount == 1)
            {
                using var document =
                    JsonDocument.Parse("{}");

                return Task.FromResult<LlmResponse>(
                    new LlmToolCallResponse(
                        "test_tool",
                        document.RootElement.Clone(),
                        "call_test_1",
                        "resp_test_1"));
            }

            return Task.FromResult<LlmResponse>(
                new LlmTextResponse(
                    "Tool returned 42.",
                    "resp_test_2"));
        }
    }

    [Fact]
    public async Task RunAsync_WhenLlmCallsTool_ShouldExecuteToolAndReturnFinalText()
    {
        var llm =
            new ToolCallingFakeLlmProvider();

        var registry =
            new FakeToolRegistry();

        var orchestrator =
            new AgentOrchestrator(
                llm,
                registry);

        var result = await orchestrator.RunAsync("Use the test tool.", TestContext.Current.CancellationToken);

        Assert.Equal(
            "Tool returned 42.",
            result.Content);
    }
}