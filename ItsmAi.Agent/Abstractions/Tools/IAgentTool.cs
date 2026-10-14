using System.Text.Json;

namespace ItsmAi.Agent.Abstractions.Tools;

public interface IAgentTool
{
    string Name { get; }

    string Description { get; }

    string ParametersJsonSchema { get; }

    AgentToolAccess Access { get; }

    Task<AgentToolResult> ExecuteAsync(
        JsonElement arguments,
        CancellationToken cancellationToken = default);
}