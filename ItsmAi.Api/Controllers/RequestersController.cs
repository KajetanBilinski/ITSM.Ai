using ItsmAi.Api.Contracts.Requesters;
using ItsmAi.Application.Requesters.Create;
using Microsoft.AspNetCore.Mvc;

namespace ItsmAi.Api.Controllers;

[ApiController]
[Route("api/requesters")]
public class RequestersController : ControllerBase
{
    private readonly CreateRequesterHandler _createRequesterHandler;

    public RequestersController(
        CreateRequesterHandler createRequesterHandler)
    {
        _createRequesterHandler = createRequesterHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateRequesterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRequesterCommand(
            request.Name,
            request.Email);

        var requesterId = await _createRequesterHandler.HandleAsync(
            command,
            cancellationToken);

        return Created(
            $"/api/requesters/{requesterId}",
            new
            {
                id = requesterId
            });
    }
}