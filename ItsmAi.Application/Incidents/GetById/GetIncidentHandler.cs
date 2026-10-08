using ItsmAi.Application.Contracts;

namespace ItsmAi.Application.Incidents.GetById;

public class GetIncidentHandler
{
    private readonly IIncidentRepository _incidentRepository;

    public GetIncidentHandler(
        IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<GetIncidentResult?> HandleAsync(
        GetIncidentQuery query,
        CancellationToken cancellationToken = default)
    {
        var incident = await _incidentRepository.GetByIdAsync(
            query.Id,
            cancellationToken);

        if (incident is null)
            return null;

        var comments = incident.Comments
            .OrderBy(x => x.CreatedAt)
            .Select(x => new IncidentCommentResult(
                x.Id,
                x.Content,
                x.CreatedAt))
            .ToList();

        return new GetIncidentResult(
            incident.Id,
            incident.Number,
            incident.Title,
            incident.Description,
            incident.Status,
            incident.Priority,
            incident.CreatedAt,
            comments);
    }
}