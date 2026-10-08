using ItsmAi.Application.Contracts;
using ItsmAi.Domain.Entities;
using ItsmAi.Domain.Exceptions;

namespace ItsmAi.Application.Incidents.Create;

public class CreateIncidentHandler
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IRequesterRepository _requesterRepository;

    public CreateIncidentHandler(
        IIncidentRepository incidentRepository,
        IRequesterRepository requesterRepository)
    {
        _incidentRepository = incidentRepository;
        _requesterRepository = requesterRepository;
    }

    public async Task<Guid> HandleAsync(
    CreateIncidentCommand command,
    CancellationToken cancellationToken = default)
    {
        var requesterExists =
            await _requesterRepository.ExistsByIdAsync(
                command.RequesterId,
                cancellationToken);

        if (!requesterExists)
        {
            throw new DomainException(
                "Requester does not exist.");
        }

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