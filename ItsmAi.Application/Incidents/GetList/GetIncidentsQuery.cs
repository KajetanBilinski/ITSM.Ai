using ItsmAi.Domain.Enums;

namespace ItsmAi.Application.Incidents.GetList;

public record GetIncidentsQuery(
    IncidentStatus? Status,
    IncidentPriority? Priority,
    int Page = 1,
    int PageSize = 20);