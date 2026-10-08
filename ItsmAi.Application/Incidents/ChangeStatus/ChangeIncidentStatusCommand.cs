using ItsmAi.Domain.Enums;

namespace ItsmAi.Application.Incidents.ChangeStatus;

public record ChangeIncidentStatusCommand(
    Guid IncidentId,
    IncidentStatus Status);