using ItsmAi.Domain.Enums;

namespace ItsmAi.Application.Incidents.GetById;

public record IncidentCommentResult(
    Guid Id,
    string Content,
    IncidentCommentAuthorType AuthorType,
    DateTime CreatedAt);