using System.Text.Json;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Agent.Tools;
using Xunit;

namespace ItsmAi.Tests.Agent;

public class AgentToolRegistryTests
{
    [Fact]
    public void Get_WhenToolExists_ShouldReturnTool()
    {
        var tool = new FakeTool();

        var registry = new AgentToolRegistry(
            new[]
            {
                tool
            });

        var result = registry.Get(
            "test_tool");
        Assert.Same(
            tool,
            result);
    }

    [Fact]
    public void Get_WhenToolNameHasDifferentCase_ShouldReturnTool()
    {
        var tool = new FakeTool();

        var registry = new AgentToolRegistry(
            new[]
            {
                tool
            });
        var result = registry.Get(
            "TEST_TOOL");
        Assert.Same(
            tool,
            result);
    }

    [Fact]
    public void Get_WhenToolDoesNotExist_ShouldReturnNull()
    {
        var registry = new AgentToolRegistry(
            Array.Empty<IAgentTool>());

        var result = registry.Get(
            "unknown_tool");
        Assert.Null(result);
    }

    [Fact]
    public void Constructor_WhenDuplicateToolNamesExist_ShouldThrow()
    {
        var firstTool =
            new FakeTool();

        var secondTool =
            new FakeTool();

        Assert.Throws<ArgumentException>(
            () => new AgentToolRegistry(
                new IAgentTool[]
                {
                    firstTool,
                    secondTool
                }));
    }

    private sealed class FakeTool
        : IAgentTool
    {
        public string Name =>
            "test_tool";

        public string Description =>
            "Tool used only for tests.";

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
                new AgentToolResult(true));
        }
    }
}