using ItsmAi.Domain.Enums;

namespace ItsmAi.Agent.Tools.Incidents;

public record SearchIncidentsToolArguments(
    IncidentStatus? Status,
    IncidentPriority? Priority,
    int Page = 1,
    int PageSize = 20);