using System.Text.Json;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Agent.Tools;

namespace ItsmAi.Tests.Agent;

public class AgentToolRegistryTests
{
    [Fact]
    public void Get_WhenToolExists_ShouldReturnTool()
    {
        var tool = new FakeTool();

        var registry = new AgentToolRegistry(
            new[] { tool });

        var result = registry.Get("test_tool");

        Assert.Same(tool, result);
    }

    [Fact]
    public void Get_WhenToolDoesNotExist_ShouldReturnNull()
    {
        var registry = new AgentToolRegistry(
            Array.Empty<IAgentTool>());

        var result = registry.Get("unknown_tool");

        Assert.Null(result);
    }

    private class FakeTool : IAgentTool
    {
        public string Name => "test_tool";

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