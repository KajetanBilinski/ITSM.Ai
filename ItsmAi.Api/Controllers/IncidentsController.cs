using ItsmAi.Api.Models.Incidents;
using ItsmAi.Application.Incidents.Create;
using ItsmAi.Application.Incidents.GetById;
using Microsoft.AspNetCore.Mvc;

namespace ItsmAi.Api.Controllers;

[ApiController]
[Route("api/incidents")]
public class IncidentsController : ControllerBase
{
    private readonly CreateIncidentHandler _createIncidentHandler;
    private readonly GetIncidentHandler _getIncidentHandler;
    public IncidentsController(
        CreateIncidentHandler createIncidentHandler,
        GetIncidentHandler getIncidentHandler)
    {
        _createIncidentHandler = createIncidentHandler;
        _getIncidentHandler = getIncidentHandler;
    }
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateIncidentCommand(
            request.Title,
            request.Description,
            request.Priority);

        var incidentId = await _createIncidentHandler.HandleAsync(
            command,
            cancellationToken);

        return Created(
            $"/api/incidents/{incidentId}",
            new
            {
                id = incidentId
            });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var query = new GetIncidentQuery(id);

        var incident = await _getIncidentHandler.HandleAsync(
            query,
            cancellationToken);

        if (incident is null)
            return NotFound();

        return Ok(incident);
    }
}