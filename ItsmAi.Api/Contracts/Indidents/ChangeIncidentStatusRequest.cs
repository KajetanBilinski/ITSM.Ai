using ItsmAi.Domain.Enums;

namespace ItsmAi.Api.Contracts.Incidents;

public record ChangeIncidentStatusRequest(
    IncidentStatus Status);