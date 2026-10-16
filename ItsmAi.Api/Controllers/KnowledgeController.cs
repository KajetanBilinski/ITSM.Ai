using ItsmAi.Api.Contracts.Knowledge;
using ItsmAi.Application.Knowledge.IngestDocument;
using ItsmAi.Application.Knowledge.Search;
using Microsoft.AspNetCore.Mvc;

namespace ItsmAi.Api.Controllers;

[ApiController]
[Route("api/knowledge")]
public class KnowledgeController : ControllerBase
{
    private readonly IngestKnowledgeDocumentHandler _ingestHandler;
    private readonly SearchKnowledgeHandler _searchHandler;

    public KnowledgeController(
        IngestKnowledgeDocumentHandler ingestHandler,
        SearchKnowledgeHandler searchHandler)
    {
        _ingestHandler = ingestHandler;
        _searchHandler = searchHandler;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
    SearchKnowledgeRequest request,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest(new
            {
                error = "Query is required."
            });
        }

        var result =
            await _searchHandler.HandleAsync(
                new SearchKnowledgeQuery(
                    request.Query,
                    request.Limit),
                cancellationToken);

        return Ok(result);
    }

    [HttpPost("documents")]
    public async Task<IActionResult> CreateDocument(
        CreateKnowledgeDocumentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new
            {
                error = "Title is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest(new
            {
                error = "Content is required."
            });
        }

        var result =
            await _ingestHandler.HandleAsync(
                new IngestKnowledgeDocumentCommand(
                    request.Title,
                    request.Content),
                cancellationToken);

        return Created(
            $"/api/knowledge/documents/{result.DocumentId}",
            result);
    }
}