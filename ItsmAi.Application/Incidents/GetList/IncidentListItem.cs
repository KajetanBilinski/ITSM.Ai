using ItsmAi.Domain.Enums;

namespace ItsmAi.Application.Incidents.GetList;

public record IncidentListItem(
    Guid Id,
    string Number,
    string Title,
    IncidentStatus Status,
    IncidentPriority Priority,
    DateTime CreatedAt);