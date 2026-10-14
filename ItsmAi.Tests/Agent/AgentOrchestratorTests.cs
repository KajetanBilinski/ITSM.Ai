using System.Text.Json;
using ItsmAi.Agent.Abstractions.Llm;
using ItsmAi.Agent.Abstractions.Observability;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Agent.Orchestration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ItsmAi.Tests.Agent;

public class AgentOrchestratorTests
{
    [Fact]
    public async Task RunAsync_WhenLlmReturnsText_ShouldReturnText()
    {
        var llmProvider = new TextLlmProvider(
            "Hello from agent.");

        var toolRegistry = new EmptyToolRegistry();
        var runStore = new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        var result = await orchestrator.RunAsync(
            "Hello", TestContext.Current.CancellationToken);

        Assert.Equal(
            "Hello from agent.",
            result.Content);
    }

    [Fact]
    public async Task RunAsync_ShouldStartAndCompleteAgentRun()
    {
        var llmProvider = new TextLlmProvider(
            "Final answer.");

        var toolRegistry = new EmptyToolRegistry();
        var runStore = new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        var result = await orchestrator.RunAsync(
            "Hello", TestContext.Current.CancellationToken);

        Assert.Equal(
            1,
            runStore.StartCallCount);

        Assert.Equal(
            "Hello",
            runStore.StartedUserMessage);

        Assert.Equal(
            1,
            runStore.CompleteCallCount);

        Assert.Equal(
            "Final answer.",
            runStore.FinalResponse);

        Assert.Equal(
            0,
            runStore.FailCallCount);
    }

    [Fact]
    public async Task RunAsync_ShouldSendSystemPromptToLlm()
    {
        var llmProvider =
            new RecordingTextLlmProvider();

        var toolRegistry =
            new EmptyToolRegistry();

        var runStore =
            new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        await orchestrator.RunAsync(
            "Hello", TestContext.Current.CancellationToken);

        Assert.NotNull(
            llmProvider.Request);

        var systemMessage =
            llmProvider.Request!.Messages.First();

        Assert.Equal(
            LlmMessageRole.System,
            systemMessage.Role);

        Assert.Contains(
            "Never invent incident data",
            systemMessage.Content);
    }

    [Fact]
    public async Task RunAsync_WhenLlmCallsTool_ShouldExecuteTool()
    {
        var llmProvider =
            new SingleToolCallLlmProvider();

        var tool =
            new CountingFakeTool();

        var toolRegistry =
            new FakeToolRegistry(tool);

        var runStore =
            new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        var result = await orchestrator.RunAsync(
            "Use the test tool.", TestContext.Current.CancellationToken);

        Assert.Equal(
            "Tool completed.",
            result.Content);

        Assert.Equal(
            1,
            tool.ExecutionCount);
    }

    [Fact]
    public async Task RunAsync_WhenToolIsExecuted_ShouldSendToolResultBackToLlm()
    {
        var llmProvider =
            new RecordingToolCallLlmProvider();

        var tool =
            new CountingFakeTool();

        var toolRegistry =
            new FakeToolRegistry(tool);

        var runStore =
            new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        await orchestrator.RunAsync(
            "Use the test tool.", TestContext.Current.CancellationToken);

        Assert.Equal(
            2,
            llmProvider.Requests.Count);

        var secondRequest =
            llmProvider.Requests[1];

        var toolMessage =
            secondRequest.Messages.SingleOrDefault(
                x => x.Role == LlmMessageRole.Tool);

        Assert.NotNull(
            toolMessage);

        Assert.Equal(
            "test_tool",
            toolMessage!.ToolName);

        Assert.Contains(
            "42",
            toolMessage.Content);
    }

    [Fact]
    public async Task RunAsync_WhenToolIsExecuted_ShouldRecordToolExecution()
    {
        var llmProvider =
            new SingleToolCallLlmProvider();

        var tool =
            new CountingFakeTool();

        var toolRegistry =
            new FakeToolRegistry(tool);

        var runStore =
            new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        await orchestrator.RunAsync(
            "Use the test tool.", TestContext.Current.CancellationToken);

        Assert.Single(
            runStore.ToolExecutions);

        var execution =
            runStore.ToolExecutions[0];

        Assert.Equal(
            "test_tool",
            execution.ToolName);

        Assert.True(
            execution.Success);

        Assert.Contains(
            "42",
            execution.ResultJson);

        Assert.True(
            execution.CompletedAt >= execution.StartedAt);
    }

    [Fact]
    public async Task RunAsync_WhenLlmRequestsMultipleSequentialTools_ShouldExecuteThem()
    {
        var llmProvider =
            new SequentialToolCallLlmProvider();

        var tool =
            new CountingFakeTool();

        var toolRegistry =
            new FakeToolRegistry(tool);

        var runStore =
            new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        var result = await orchestrator.RunAsync(
            "Use the tool twice.", TestContext.Current.CancellationToken);

        Assert.Equal(
            "Finished after two tools.",
            result.Content);

        Assert.Equal(
            2,
            tool.ExecutionCount);

        Assert.Equal(
            2,
            runStore.ToolExecutions.Count);
    }

    [Fact]
    public async Task RunAsync_WhenToolCallLimitIsExceeded_ShouldThrowAndFailRun()
    {
        var llmProvider =
            new InfiniteToolCallLlmProvider();

        var tool =
            new CountingFakeTool();

        var toolRegistry =
            new FakeToolRegistry(tool);

        var runStore =
            new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => orchestrator.RunAsync(
                    "Keep using tools.", TestContext.Current.CancellationToken));

        Assert.Contains(
            "maximum number of tool calls",
            exception.Message);

        Assert.Equal(
            5,
            tool.ExecutionCount);

        Assert.Equal(
            5,
            runStore.ToolExecutions.Count);

        Assert.Equal(
            1,
            runStore.FailCallCount);

        Assert.Contains(
            "maximum number of tool calls",
            runStore.Error);
    }

    [Fact]
    public async Task RunAsync_WhenLlmRequestsUnknownTool_ShouldThrowAndFailRun()
    {
        var llmProvider =
            new UnknownToolLlmProvider();

        var toolRegistry =
            new EmptyToolRegistry();

        var runStore =
            new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => orchestrator.RunAsync(
                    "Do something.", TestContext.Current.CancellationToken));

        Assert.Contains(
            "unknown_tool",
            exception.Message);

        Assert.Equal(
            1,
            runStore.FailCallCount);

        Assert.Contains(
            "unknown_tool",
            runStore.Error);
    }

    [Fact]
    public async Task RunAsync_WhenLlmReturnsParallelToolCalls_ShouldThrowAndFailRun()
    {
        var llmProvider =
            new ParallelToolCallsLlmProvider();

        var toolRegistry =
            new EmptyToolRegistry();

        var runStore =
            new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        var exception =
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => orchestrator.RunAsync(
                    "Use multiple tools.", TestContext.Current.CancellationToken));

        Assert.Equal(
            "Parallel tool calls are not supported yet.",
            exception.Message);

        Assert.Equal(
            1,
            runStore.FailCallCount);
    }

    private static AgentOrchestrator CreateOrchestrator(
        ILlmProvider llmProvider,
        IAgentToolRegistry toolRegistry,
        IAgentRunStore runStore)
    {
        return new AgentOrchestrator(
            llmProvider,
            toolRegistry,
            runStore,
            NullLogger<AgentOrchestrator>.Instance);
    }

    private sealed class TextLlmProvider
        : ILlmProvider
    {
        private readonly string _response;

        public TextLlmProvider(
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

    private sealed class RecordingTextLlmProvider
        : ILlmProvider
    {
        public LlmRequest? Request { get; private set; }

        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            Request = request;

            return Task.FromResult(
                new LlmResponse(
                    "Hello.",
                    Array.Empty<LlmToolCall>()));
        }
    }

    private sealed class SingleToolCallLlmProvider
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
                return Task.FromResult(
                    CreateToolCallResponse());
            }

            return Task.FromResult(
                new LlmResponse(
                    "Tool completed.",
                    Array.Empty<LlmToolCall>()));
        }
    }

    private sealed class RecordingToolCallLlmProvider
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
                return Task.FromResult(
                    CreateToolCallResponse());
            }

            return Task.FromResult(
                new LlmResponse(
                    "Done.",
                    Array.Empty<LlmToolCall>()));
        }
    }

    private sealed class SequentialToolCallLlmProvider
        : ILlmProvider
    {
        private int _callCount;

        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            _callCount++;

            if (_callCount <= 2)
            {
                return Task.FromResult(
                    CreateToolCallResponse());
            }

            return Task.FromResult(
                new LlmResponse(
                    "Finished after two tools.",
                    Array.Empty<LlmToolCall>()));
        }
    }

    private sealed class InfiniteToolCallLlmProvider
        : ILlmProvider
    {
        public Task<LlmResponse> CompleteAsync(
            LlmRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                CreateToolCallResponse());
        }
    }

    private sealed class UnknownToolLlmProvider
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

    private sealed class ParallelToolCallsLlmProvider
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

            return Task.FromResult(
                new LlmResponse(
                    string.Empty,
                    new[]
                    {
                        new LlmToolCall(
                            "first_tool",
                            firstDocument.RootElement.Clone()),

                        new LlmToolCall(
                            "second_tool",
                            secondDocument.RootElement.Clone())
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

    private sealed class CountingFakeTool
        : IAgentTool
    {
        public int ExecutionCount { get; private set; }

        public string Name =>
            "test_tool";

        public string Description =>
            "Tool used for agent tests.";

        public AgentToolAccess Access =>
            AgentToolAccess.Read;

        public string ParametersJsonSchema =>
            """
            {
              "type": "object",
              "properties": {
                "value": {
                  "type": "integer"
                }
              },
              "additionalProperties": false
            }
            """;

        public Task<AgentToolResult> ExecuteAsync(
            JsonElement arguments,
            CancellationToken cancellationToken = default)
        {
            ExecutionCount++;

            return Task.FromResult(
                new AgentToolResult(
                    true,
                    new
                    {
                        value = 42,
                        executionCount = ExecutionCount
                    }));
        }
    }

    private sealed class FakeAgentRunStore
        : IAgentRunStore
    {
        private readonly Guid _agentRunId =
            Guid.NewGuid();

        public int StartCallCount { get; private set; }

        public int CompleteCallCount { get; private set; }

        public int FailCallCount { get; private set; }

        public string? StartedUserMessage { get; private set; }

        public string? FinalResponse { get; private set; }

        public string Error { get; private set; } =
            string.Empty;

        public List<ToolExecutionRecord> ToolExecutions { get; } =
            [];

        public Task<Guid> StartAsync(
            string userMessage,
            CancellationToken cancellationToken = default)
        {
            StartCallCount++;
            StartedUserMessage = userMessage;

            return Task.FromResult(
                _agentRunId);
        }

        public Task RecordToolExecutionAsync(
            Guid agentRunId,
            ToolExecutionRecord execution,
            CancellationToken cancellationToken = default)
        {
            ToolExecutions.Add(
                execution);

            return Task.CompletedTask;
        }

        public Task CompleteAsync(
            Guid agentRunId,
            string finalResponse,
            CancellationToken cancellationToken = default)
        {
            CompleteCallCount++;
            FinalResponse = finalResponse;

            return Task.CompletedTask;
        }

        public Task FailAsync(
            Guid agentRunId,
            string error,
            CancellationToken cancellationToken = default)
        {
            FailCallCount++;
            Error = error;

            return Task.CompletedTask;
        }
    }

    private static LlmResponse CreateToolCallResponse()
    {
        using var document =
            JsonDocument.Parse(
                """
                {
                  "value": 42
                }
                """);

        return new LlmResponse(
            string.Empty,
            new[]
            {
                new LlmToolCall(
                    "test_tool",
                    document.RootElement.Clone())
            });
    }

    [Fact]
    public async Task RunAsync_WhenLlmRequestsWriteTool_ShouldRequireApprovalWithoutExecutingTool()
    {
        // Arrange
        var llmProvider =
            new SingleToolCallLlmProvider();

        var tool =
            new WriteFakeTool();

        var toolRegistry =
            new FakeToolRegistry(tool);

        var runStore =
            new FakeAgentRunStore();

        var orchestrator = CreateOrchestrator(
            llmProvider,
            toolRegistry,
            runStore);

        var result = await orchestrator.RunAsync(
            "Perform a write operation.", TestContext.Current.CancellationToken);

        Assert.NotNull(
            result.ApprovalRequired);

        Assert.Equal(
            "test_tool",
            result.ApprovalRequired!.ToolName);

        Assert.Equal(
            0,
            tool.ExecutionCount);

        Assert.Equal(
            1,
            runStore.CompleteCallCount);
    }

    private sealed class WriteFakeTool
    : IAgentTool
    {
        public int ExecutionCount { get; private set; }

        public string Name =>
            "test_tool";

        public string Description =>
            "Test write tool.";

        public AgentToolAccess Access =>
            AgentToolAccess.Write;

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
            ExecutionCount++;

            return Task.FromResult(
                new AgentToolResult(
                    true));
        }
    }
}