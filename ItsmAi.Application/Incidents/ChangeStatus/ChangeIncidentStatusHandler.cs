using ItsmAi.Application.Contracts;
using ItsmAi.Domain.Enums;
using ItsmAi.Domain.Exceptions;

namespace ItsmAi.Application.Incidents.ChangeStatus;

public class ChangeIncidentStatusHandler
{
    private readonly IIncidentRepository _incidentRepository;

    public ChangeIncidentStatusHandler(
        IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<bool> HandleAsync(
        ChangeIncidentStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var incident = await _incidentRepository.GetForUpdateAsync(
            command.IncidentId,
            cancellationToken);

        if (incident is null)
            return false;

        incident.ChangeStatus(command.Status);

        await _incidentRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}