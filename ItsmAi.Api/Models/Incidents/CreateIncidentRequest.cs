using ItsmAi.Domain.Enums;

namespace ItsmAi.Api.Models.Incidents;

public record CreateIncidentRequest(
    string Title,
    string Description,
    IncidentPriority Priority);