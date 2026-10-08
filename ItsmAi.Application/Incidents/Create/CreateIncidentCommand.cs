using ItsmAi.Domain.Enums;

namespace ItsmAi.Application.Incidents.Create;

public record CreateIncidentCommand(
    string Title,
    string Description,
    IncidentPriority Priority);