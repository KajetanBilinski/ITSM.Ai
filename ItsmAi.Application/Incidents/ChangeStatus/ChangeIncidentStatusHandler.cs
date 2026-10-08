using ItsmAi.Application.Contracts;
using ItsmAi.Domain.Enums;

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

        switch (command.Status)
        {
            case IncidentStatus.InProgress:
                incident.StartProgress();
                break;

            case IncidentStatus.Resolved:
                incident.Resolve();
                break;

            case IncidentStatus.Closed:
                incident.Close();
                break;

            default:
                throw new InvalidOperationException(
                    "Requested incident status transition is not supported.");
        }

        await _incidentRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}