using ItsmAi.Domain.Enums;

namespace ItsmAi.Api.Models.Incidents;

public record CreateIncidentRequest(
    Guid RequesterId,
    string Title,
    string Description,
    IncidentPriority Priority);