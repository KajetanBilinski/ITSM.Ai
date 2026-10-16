using ItsmAi.Api.Contracts.Knowledge;
using ItsmAi.Application.Knowledge.IngestDocument;
using Microsoft.AspNetCore.Mvc;

namespace ItsmAi.Api.Controllers;

[ApiController]
[Route("api/knowledge")]
public class KnowledgeController : ControllerBase
{
    private readonly IngestKnowledgeDocumentHandler _handler;

    public KnowledgeController(
        IngestKnowledgeDocumentHandler handler)
    {
        _handler = handler;
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
            await _handler.HandleAsync(
                new IngestKnowledgeDocumentCommand(
                    request.Title,
                    request.Content),
                cancellationToken);

        return Created(
            $"/api/knowledge/documents/{result.DocumentId}",
            result);
    }
}