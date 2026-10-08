using ItsmAi.Application.Contracts;

namespace ItsmAi.Application.Incidents.GetById;

public class GetIncidentHandler
{
    private readonly IIncidentRepository _incidentRepository;
    private readonly IRequesterRepository _requesterRepository;

    public GetIncidentHandler(
        IIncidentRepository incidentRepository,
        IRequesterRepository requesterRepository)
    {
        _incidentRepository = incidentRepository;
        _requesterRepository = requesterRepository;
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

        var requester = await _requesterRepository.GetByIdAsync(
            incident.RequesterId,
            cancellationToken);

        if (requester is null)
        {
            throw new InvalidOperationException(
                "Incident references a requester that does not exist.");
        }

        var requesterResult = new RequesterResult(
            requester.Id,
            requester.Name,
            requester.Email);

        var comments = incident.Comments
            .OrderBy(x => x.CreatedAt)
            .Select(x => new IncidentCommentResult(
                x.Id,
                x.Content,
                x.AuthorType,
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
            requesterResult,
            comments);
    }
}