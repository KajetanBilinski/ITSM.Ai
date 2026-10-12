using System.Text.Json;
using ItsmAi.Agent.Abstractions.Agent;
using ItsmAi.Agent.Abstractions.Llm;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Agent.Orchestration;
using Xunit;

namespace ItsmAi.Tests.Agent;

public class AgentOrchestratorTests
{
    [Fact]
    public async Task RunAsync_WhenLlmReturnsText_ShouldReturnText()
    {
        var llmProvider = new TextFakeLlmProvider(
            "Hello from agent.");

        var toolRegistry = new EmptyToolRegistry();

        var orchestrator = new AgentOrchestrator(
            llmProvider,
            toolRegistry);

        var result = await orchestrator.RunAsync(
            "Hello", TestContext.Current.CancellationToken);

        Assert.Equal(
            "Hello from agent.",
            result.Content);
    }

    [Fact]
    public async Task RunAsync_WhenLlmCallsTool_ShouldExecuteToolAndReturnFinalText()
    {
        var llmProvider =
            new ToolCallingFakeLlmProvider();

        var tool = new FakeTool();

        var toolRegistry =
            new FakeToolRegistry(tool);

        var orchestrator = new AgentOrchestrator(
            llmProvider,
            toolRegistry);

        var result = await orchestrator.RunAsync(
            "Use the test tool.", TestContext.Current.CancellationToken);

        Assert.Equal(
            "Tool returned 42.",
            result.Content);

        Assert.True(tool.WasExecuted);
    }

    [Fact]
    public async Task RunAsync_WhenToolIsExecuted_ShouldSendToolResultBackToLlm()
    {
        var llmProvider =
            new RecordingToolCallingFakeLlmProvider();

        var tool = new FakeTool();

        var toolRegistry =
            new FakeToolRegistry(tool);

        var orchestrator = new AgentOrchestrator(
            llmProvider,
            toolRegistry);

        await orchestrator.RunAsync(
            "Use the test tool.", TestContext.Current.CancellationToken);

        Assert.Equal(
            2,
            llmProvider.Requests.Count);

        var secondRequest =
            llmProvider.Requests[1];

        var toolMessage = secondRequest.Messages
            .SingleOrDefault(
                x => x.Role == LlmMessageRole.Tool);

        Assert.NotNull(toolMessage);

        Assert.Equal(
            "test_tool",
            toolMessage.ToolName);

        Assert.Contains(
            "42",
            toolMessage.Content);
    }

    [Fact]
    public async Task RunAsync_WhenLlmRequestsUnknownTool_ShouldThrow()
    {
        var llmProvider =
            new UnknownToolFakeLlmProvider();

        var toolRegistry =
            new EmptyToolRegistry();

        var orchestrator = new AgentOrchestrator(
            llmProvider,
            toolRegistry);
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => orchestrator.RunAsync(
                    "Do something.", TestContext.Current.CancellationToken));
        Assert.Contains(
            "unknown_tool",
            exception.Message);
    }

    [Fact]
    public async Task RunAsync_WhenLlmReturnsMultipleToolCalls_ShouldThrow()
    {
        var llmProvider =
            new MultipleToolCallsFakeLlmProvider();

        var toolRegistry =
            new EmptyToolRegistry();

        var orchestrator = new AgentOrchestrator(
            llmProvider,
            toolRegistry);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => orchestrator.RunAsync(
                    "Use multiple tools.", TestContext.Current.CancellationToken));
        Assert.Equal(
            "Parallel tool calls are not supported yet.",
            exception.Message);
    }

    [Fact]
    public async Task RunAsync_WhenLlmRequestsAnotherToolAfterExecution_ShouldThrow()
    {
        var llmProvider =
            new RepeatedToolCallFakeLlmProvider();

        var tool = new FakeTool();

        var toolRegistry =
            new FakeToolRegistry(tool);

        var orchestrator = new AgentOrchestrator(
            llmProvider,
            toolRegistry);
        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => orchestrator.RunAsync(
                    "Use the tool multiple times.", TestContext.Current.CancellationToken));
        Assert.Equal(
            "Additional tool calls are not supported yet.",
            exception.Message);
    }

    private sealed class TextFakeLlmProvider
        : ILlmProvider
    {
        private readonly string _response;

        public TextFakeLlmProvider(
            string response)
        {
            _response = response;
        }

        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new LlmResponse(
                    _response,
                    Array.Empty<LlmToolCall>()));
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
                    JsonDocument.Parse(
                        """
                        {
                          "value": 42
                        }
                        """);

                var toolCall = new LlmToolCall(
                    "test_tool",
                    document.RootElement.Clone());

                return Task.FromResult(
                    new LlmResponse(
                        string.Empty,
                        new[]
                        {
                            toolCall
                        }));
            }

            return Task.FromResult(
                new LlmResponse(
                    "Tool returned 42.",
                    Array.Empty<LlmToolCall>()));
        }
    }

    private sealed class RecordingToolCallingFakeLlmProvider
        : ILlmProvider
    {
        private int _callCount;

        public List<LlmRequest> Requests { get; } = [];

        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(request);

            _callCount++;

            if (_callCount == 1)
            {
                using var document =
                    JsonDocument.Parse(
                        """
                        {
                          "value": 42
                        }
                        """);

                return Task.FromResult(
                    new LlmResponse(
                        string.Empty,
                        new[]
                        {
                            new LlmToolCall(
                                "test_tool",
                                document.RootElement.Clone())
                        }));
            }

            return Task.FromResult(
                new LlmResponse(
                    "Done.",
                    Array.Empty<LlmToolCall>()));
        }
    }

    private sealed class UnknownToolFakeLlmProvider
        : ILlmProvider
    {
        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            using var document =
                JsonDocument.Parse("{}");

            return Task.FromResult(
                new LlmResponse(
                    string.Empty,
                    new[]
                    {
                        new LlmToolCall(
                            "unknown_tool",
                            document.RootElement.Clone())
                    }));
        }
    }

    private sealed class MultipleToolCallsFakeLlmProvider
        : ILlmProvider
    {
        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            using var firstDocument =
                JsonDocument.Parse("{}");

            using var secondDocument =
                JsonDocument.Parse("{}");

            var firstToolCall =
                new LlmToolCall(
                    "first_tool",
                    firstDocument.RootElement.Clone());

            var secondToolCall =
                new LlmToolCall(
                    "second_tool",
                    secondDocument.RootElement.Clone());

            return Task.FromResult(
                new LlmResponse(
                    string.Empty,
                    new[]
                    {
                        firstToolCall,
                        secondToolCall
                    }));
        }
    }

    private sealed class RepeatedToolCallFakeLlmProvider
        : ILlmProvider
    {
        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            using var document =
                JsonDocument.Parse("{}");

            return Task.FromResult(
                new LlmResponse(
                    string.Empty,
                    new[]
                    {
                        new LlmToolCall(
                            "test_tool",
                            document.RootElement.Clone())
                    }));
        }
    }

    private sealed class EmptyToolRegistry
        : IAgentToolRegistry
    {
        public IReadOnlyCollection<IAgentTool> Tools =>
            Array.Empty<IAgentTool>();

        public IAgentTool? Get(
            string name)
        {
            return null;
        }
    }

    private sealed class FakeToolRegistry
        : IAgentToolRegistry
    {
        private readonly IAgentTool _tool;

        public FakeToolRegistry(
            IAgentTool tool)
        {
            _tool = tool;
        }

        public IReadOnlyCollection<IAgentTool> Tools =>
            new[]
            {
                _tool
            };

        public IAgentTool? Get(
            string name)
        {
            return string.Equals(
                name,
                _tool.Name,
                StringComparison.OrdinalIgnoreCase)
                    ? _tool
                    : null;
        }
    }

    private sealed class FakeTool
        : IAgentTool
    {
        public bool WasExecuted { get; private set; }

        public string Name =>
            "test_tool";

        public string Description =>
            "Tool used only for tests.";

        public string ParametersJsonSchema =>
            """
            {
              "type": "object",
              "properties": {
                "value": {
                  "type": "integer"
                }
              }
            }
            """;

        public Task<AgentToolResult> ExecuteAsync(
            JsonElement arguments,
            CancellationToken cancellationToken = default)
        {
            WasExecuted = true;

            return Task.FromResult(
                new AgentToolResult(
                    true,
                    new
                    {
                        value = 42
                    }));
        }
    }
}