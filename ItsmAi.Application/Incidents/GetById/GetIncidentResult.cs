using ItsmAi.Domain.Enums;

namespace ItsmAi.Application.Incidents.GetById;

public record GetIncidentResult(
    Guid Id,
    string Number,
    string Title,
    string Description,
    IncidentStatus Status,
    IncidentPriority Priority,
    DateTime CreatedAt);