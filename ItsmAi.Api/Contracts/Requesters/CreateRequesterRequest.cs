namespace ItsmAi.Api.Contracts.Requesters;

public record CreateRequesterRequest(
    string Name,
    string Email);