using ItsmAi.Application.Incidents.GetById;
using ItsmAi.Domain.Enums;

public record GetIncidentResult(
    Guid Id,
    string Number,
    string Title,
    string Description,
    IncidentStatus Status,
    IncidentPriority Priority,
    DateTime CreatedAt,
    RequesterResult Requester,
    IReadOnlyList<IncidentCommentResult> Comments);