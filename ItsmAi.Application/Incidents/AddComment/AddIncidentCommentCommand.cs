namespace ItsmAi.Application.Incidents.AddComment;

public record AddIncidentCommentCommand(
    Guid IncidentId,
    string Content);