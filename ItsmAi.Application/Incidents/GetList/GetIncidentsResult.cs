namespace ItsmAi.Application.Incidents.GetList;

public record GetIncidentsResult(
    IReadOnlyList<IncidentListItem> Items,
    int Page,
    int PageSize,
    int TotalCount);