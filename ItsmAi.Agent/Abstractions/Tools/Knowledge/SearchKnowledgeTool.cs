using System.Text.Json;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Application.Knowledge.Search;

namespace ItsmAi.Agent.Tools.Knowledge;

public class SearchKnowledgeTool : IAgentTool
{
    private readonly SearchKnowledgeHandler _handler;

    public SearchKnowledgeTool(
        SearchKnowledgeHandler handler)
    {
        _handler = handler;
    }

    public string Name =>
        "search_knowledge";

    public string Description =>
        "Searches the ITSM knowledge base for troubleshooting procedures, instructions and relevant support documentation using semantic search.";

    public AgentToolAccess Access =>
        AgentToolAccess.Read;

    public string ParametersJsonSchema =>
        """
        {
          "type": "object",
          "properties": {
            "query": {
              "type": "string",
              "minLength": 1,
              "description": "A semantic search query describing the problem or information to find in the ITSM knowledge base."
            },
            "limit": {
              "type": "integer",
              "minimum": 1,
              "maximum": 10,
              "description": "Maximum number of knowledge chunks to return."
            }
          },
          "required": [
            "query"
          ],
          "additionalProperties": false
        }
        """;

    public async Task<AgentToolResult> ExecuteAsync(
        JsonElement arguments,
        CancellationToken cancellationToken = default)
    {
        SearchKnowledgeToolArguments? toolArguments;

        try
        {
            toolArguments =
                arguments.Deserialize<SearchKnowledgeToolArguments>(
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

        if (string.IsNullOrWhiteSpace(
            toolArguments.Query))
        {
            return new AgentToolResult(
                false,
                Error: "Knowledge search query is required.");
        }

        var result =
            await _handler.HandleAsync(
                new SearchKnowledgeQuery(
                    toolArguments.Query,
                    toolArguments.Limit),
                cancellationToken);

        return new AgentToolResult(
            true,
            Data: result);
    }
}