using System.Text.Json;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Application.Incidents.AddComment;
using ItsmAi.Domain.Enums;

namespace ItsmAi.Agent.Tools.Incidents;

public class AddIncidentCommentTool : IAgentTool
{
    private readonly AddIncidentCommentHandler _handler;

    public AddIncidentCommentTool(
        AddIncidentCommentHandler handler)
    {
        _handler = handler;
    }

    public string Name =>
        "add_incident_comment";

    public string Description =>
        "Adds a comment to an existing ITSM incident. This operation modifies ITSM data.";

    public AgentToolAccess Access =>
        AgentToolAccess.Write;

    public string ParametersJsonSchema =>
        """
        {
          "type": "object",
          "properties": {
            "incidentId": {
              "type": "string",
              "format": "uuid",
              "description": "The ID of the incident."
            },
            "content": {
              "type": "string",
              "minLength": 1,
              "maxLength": 2000,
              "description": "The comment to add to the incident."
            }
          },
          "required": [
            "incidentId",
            "content"
          ],
          "additionalProperties": false
        }
        """;

    public async Task<AgentToolResult> ExecuteAsync(
        JsonElement arguments,
        CancellationToken cancellationToken = default)
    {
        AddIncidentCommentToolArguments? toolArguments;

        try
        {
            toolArguments =
                arguments.Deserialize<AddIncidentCommentToolArguments>(
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

        if (toolArguments is null)
        {
            return new AgentToolResult(
                false,
                Error: "Invalid tool arguments.");
        }

        if (toolArguments.IncidentId == Guid.Empty)
        {
            return new AgentToolResult(
                false,
                Error: "IncidentId is required.");
        }

        if (string.IsNullOrWhiteSpace(
            toolArguments.Content))
        {
            return new AgentToolResult(
                false,
                Error: "Comment content is required.");
        }

        var command =
            new AddIncidentCommentCommand(
                toolArguments.IncidentId,
                toolArguments.Content,
                IncidentCommentAuthorType.AiAgent);

        var commentId =
            await _handler.HandleAsync(
                command,
                cancellationToken);

        if (commentId is null)
        {
            return new AgentToolResult(
                false,
                Error: "Incident not found.");
        }

        return new AgentToolResult(
            true,
            Data: new
            {
                CommentId = commentId.Value
            });
    }
}