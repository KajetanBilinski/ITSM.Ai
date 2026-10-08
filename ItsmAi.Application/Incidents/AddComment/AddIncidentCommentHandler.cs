using ItsmAi.Application.Contracts;

namespace ItsmAi.Application.Incidents.AddComment;

public class AddIncidentCommentHandler
{
    private readonly IIncidentRepository _incidentRepository;

    public AddIncidentCommentHandler(
        IIncidentRepository incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<Guid?> HandleAsync(
        AddIncidentCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        var incident = await _incidentRepository.GetForUpdateAsync(
            command.IncidentId,
            cancellationToken);

        if (incident is null)
            return null;

        var comment = incident.AddComment(command.Content);

        await _incidentRepository.AddCommentAsync(
            comment,
            cancellationToken);

        await _incidentRepository.SaveChangesAsync(
            cancellationToken);

        return comment.Id;
    }
}