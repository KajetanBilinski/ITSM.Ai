using System.Text.Json;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Application.Incidents.GetById;

namespace ItsmAi.Agent.Tools.Incidents;

public class GetIncidentTool : IAgentTool
{
    private readonly GetIncidentHandler _getIncidentHandler;

    public GetIncidentTool(
        GetIncidentHandler getIncidentHandler)
    {
        _getIncidentHandler = getIncidentHandler;
    }

    public string Name => "get_incident";

    public string Description =>
        "Gets detailed information about an ITSM incident by its ID.";

    public string ParametersJsonSchema =>
    """
    {
      "type": "object",
      "properties": {
        "incidentId": {
          "type": "string",
          "format": "uuid",
          "description": "The ID of the incident."
        }
      },
      "required": ["incidentId"],
      "additionalProperties": false
    }
    """;

    public async Task<AgentToolResult> ExecuteAsync(
        JsonElement arguments,
        CancellationToken cancellationToken = default)
    {
        GetIncidentToolArguments? toolArguments;

        try
        {
            toolArguments =
                arguments.Deserialize<GetIncidentToolArguments>(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
        catch (JsonException)
        {
            return new AgentToolResult(
                false,
                Error: "Invalid tool arguments.");
        }

        if (toolArguments is null ||
            toolArguments.IncidentId == Guid.Empty)
        {
            return new AgentToolResult(
                false,
                Error: "IncidentId is required.");
        }

        var query = new GetIncidentQuery(
            toolArguments.IncidentId);

        var incident = await _getIncidentHandler.HandleAsync(
            query,
            cancellationToken);

        if (incident is null)
        {
            return new AgentToolResult(
                false,
                Error: "Incident not found.");
        }

        return new AgentToolResult(
            true,
            Data: incident);
    }
}