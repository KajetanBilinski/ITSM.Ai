using ItsmAi.Domain.Enums;

namespace ItsmAi.Application.Incidents.Create;

public record CreateIncidentCommand(
    Guid RequesterId,
    string Title,
    string Description,
    IncidentPriority Priority);