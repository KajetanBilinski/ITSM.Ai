using System.Text.Json;
using System.Text.Json.Serialization;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Application.Incidents.GetList;

namespace ItsmAi.Agent.Tools.Incidents;

public class SearchIncidentsTool : IAgentTool
{
    private readonly GetIncidentsHandler _getIncidentsHandler;

    public SearchIncidentsTool(
        GetIncidentsHandler getIncidentsHandler)
    {
        _getIncidentsHandler = getIncidentsHandler;
    }

    public string Name => "search_incidents";

    public string Description =>
        "Searches ITSM incidents by optional status and priority filters.";

    public string ParametersJsonSchema =>
    """
    {
      "type": "object",
      "properties": {
        "status": {
          "type": "string",
          "enum": ["New", "InProgress", "Resolved", "Closed"]
        },
        "priority": {
          "type": "string",
          "enum": ["Low", "Medium", "High", "Critical"]
        },
        "page": {
          "type": "integer",
          "minimum": 1
        },
        "pageSize": {
          "type": "integer",
          "minimum": 1,
          "maximum": 100
        }
      },
      "additionalProperties": false
    }
    """;

    public async Task<AgentToolResult> ExecuteAsync(
        JsonElement arguments,
        CancellationToken cancellationToken = default)
    {
        SearchIncidentsToolArguments? toolArguments;

        try
        {
            toolArguments =
                arguments.Deserialize<SearchIncidentsToolArguments>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                    });
        }
        catch (JsonException)
        {
            return new AgentToolResult(
                false,
                Error: "Invalid tool arguments.");
        }

        if (toolArguments is null)
        {
            return new AgentToolResult(
                false,
                Error: "Invalid tool arguments.");
        }

        var query = new GetIncidentsQuery(
            toolArguments.Status,
            toolArguments.Priority,
            toolArguments.Page,
            toolArguments.PageSize);

        var result = await _getIncidentsHandler.HandleAsync(
            query,
            cancellationToken);

        return new AgentToolResult(
            true,
            Data: result);
    }
}