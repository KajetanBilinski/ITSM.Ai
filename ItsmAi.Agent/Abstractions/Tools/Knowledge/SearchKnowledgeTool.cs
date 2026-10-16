using System.Text.Json;
using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Application.Knowledge.Search;

namespace ItsmAi.Agent.Tools.Knowledge;

public class SearchKnowledgeTool : IAgentTool
{
    private const int MaxChunks = 3;
    private const int MaxExcerptCharacters = 1600;

    private readonly SearchKnowledgeHandler _handler;

    public SearchKnowledgeTool(
        SearchKnowledgeHandler handler)
    {
        _handler = handler;
    }

    public string Name => "search_knowledge";

    public string Description =>
        "Searches the internal ITSM knowledge base for relevant " +
        "troubleshooting procedures and documentation. " +
        "Returns short excerpts with source identifiers.";

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
              "description": "The technical problem or information to search for."
            },
            "limit": {
              "type": "integer",
              "minimum": 1,
              "maximum": 3,
              "description": "Maximum number of knowledge excerpts to return."
            }
          },
          "required": ["query"],
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

        if (toolArguments is null ||
            string.IsNullOrWhiteSpace(toolArguments.Query))
        {
            return new AgentToolResult(
                false,
                Error: "Knowledge search query is required.");
        }

        var limit = Math.Clamp(
            toolArguments.Limit,
            1,
            MaxChunks);

        var results = await _handler.HandleAsync(
            new SearchKnowledgeQuery(
                toolArguments.Query,
                limit),
            cancellationToken);

        var matches = results
            .Take(MaxChunks)
            .Select(result => new
            {
                SourceId =
                    $"KB:{result.DocumentId:D}:chunk:{result.ChunkIndex}",

                result.DocumentId,
                result.DocumentTitle,
                result.ChunkIndex,

                Excerpt = CreateExcerpt(result.Content),

                result.Similarity,

                Truncated =
                    result.Content.Length > MaxExcerptCharacters
            })
            .ToArray();

        return new AgentToolResult(
            true,
            Data: new
            {
                HasMatches = matches.Length > 0,
                Matches = matches
            });
    }

    private static string CreateExcerpt(string content)
    {
        if (content.Length <= MaxExcerptCharacters)
        {
            return content;
        }

        var maximumContentLength =
            MaxExcerptCharacters - 1;

        var cutoff = content.LastIndexOf(
            ' ',
            maximumContentLength);

        if (cutoff < MaxExcerptCharacters / 2)
        {
            cutoff = maximumContentLength;
        }

        return content[..cutoff].TrimEnd() + "…";
    }
}