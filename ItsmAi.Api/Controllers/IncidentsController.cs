using ItsmAi.Api.Models.Incidents;
using ItsmAi.Application.Incidents.Create;
using Microsoft.AspNetCore.Mvc;

namespace ItsmAi.Api.Controllers;

[ApiController]
[Route("api/incidents")]
public class IncidentsController : ControllerBase
{
    private readonly CreateIncidentHandler _createIncidentHandler;

    public IncidentsController(
        CreateIncidentHandler createIncidentHandler)
    {
        _createIncidentHandler = createIncidentHandler;
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
}