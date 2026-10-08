using ItsmAi.Application.Contracts;
using ItsmAi.Domain.Entities;
namespace ItsmAi.Application.Incidents.Create;

public class CreateIncidentHandler
{
    private readonly IIncidentRepository _incidentRepository;

    public CreateIncidentHandler(IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<Guid> HandleAsync(
        CreateIncidentCommand command,
        CancellationToken cancellationToken = default)
    {
        var incident = new Incident(
            command.RequesterId,
            command.Title,
            command.Description,
            command.Priority);

        await _incidentRepository.AddAsync(
            incident,
            cancellationToken);

        return incident.Id;
    }
}