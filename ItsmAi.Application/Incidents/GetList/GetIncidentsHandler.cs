using ItsmAi.Application.Contracts;

namespace ItsmAi.Application.Incidents.GetList;

public class GetIncidentsHandler
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentsHandler(
        IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<GetIncidentsResult> HandleAsync(
        GetIncidentsQuery query,
        CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);

        var pageSize = Math.Clamp(
            query.PageSize,
            1,
            100);

        var result = await _incidentRepository.GetPagedAsync(
            query.Status,
            query.Priority,
            page,
            pageSize,
            cancellationToken);

        var items = result.Items
            .Select(incident => new IncidentListItem(
                incident.Id,
                incident.Number,
                incident.Title,
                incident.Status,
                incident.Priority,
                incident.CreatedAt))
            .ToList();

        return new GetIncidentsResult(
            items,
            page,
            pageSize,
            result.TotalCount);
    }
}