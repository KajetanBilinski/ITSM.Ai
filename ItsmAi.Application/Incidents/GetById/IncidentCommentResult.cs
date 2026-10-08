namespace ItsmAi.Application.Incidents.GetById;

public record IncidentCommentResult(
    Guid Id,
    string Content,
    DateTime CreatedAt);