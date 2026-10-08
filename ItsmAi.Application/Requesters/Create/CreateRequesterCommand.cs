namespace ItsmAi.Application.Requesters.Create;

public record CreateRequesterCommand(
    string Name,
    string Email);