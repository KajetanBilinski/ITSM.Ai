using ItsmAi.Api.Contracts.Incidents;
using ItsmAi.Api.Models.Incidents;
using ItsmAi.Application.Incidents.ChangeStatus;
using ItsmAi.Application.Incidents.Create;
using ItsmAi.Application.Incidents.GetById;
using ItsmAi.Application.Incidents.GetList;
using ItsmAi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ItsmAi.Api.Controllers;

[ApiController]
[Route("api/incidents")]
public class IncidentsController : ControllerBase
{
    private readonly CreateIncidentHandler _createIncidentHandler;
    private readonly GetIncidentHandler _getIncidentHandler;
    private readonly ChangeIncidentStatusHandler _changeStatusHandler;
    private readonly GetIncidentsHandler _getIncidentsHandler;
    public IncidentsController(
        CreateIncidentHandler createIncidentHandler,
        GetIncidentHandler getIncidentHandler,
        ChangeIncidentStatusHandler changeStatusHandler,
        GetIncidentsHandler getIncidentsHandler)
    {
        _createIncidentHandler = createIncidentHandler;
        _getIncidentHandler = getIncidentHandler;
        _changeStatusHandler = changeStatusHandler;
        _getIncidentsHandler = getIncidentsHandler;
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

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
    Guid id,
    ChangeIncidentStatusRequest request,
    CancellationToken cancellationToken)
    {
        var command = new ChangeIncidentStatusCommand(
            id,
            request.Status);

        var exists = await _changeStatusHandler.HandleAsync(
            command,
            cancellationToken);

        if (!exists)
            return NotFound();

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] IncidentStatus? status,
    [FromQuery] IncidentPriority? priority,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var query = new GetIncidentsQuery(
            status,
            priority,
            page,
            pageSize);

        var result = await _getIncidentsHandler.HandleAsync(
            query,
            cancellationToken);

        return Ok(result);
    }
}