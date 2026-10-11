using ItsmAi.Agent.Abstractions.Tools;
using ItsmAi.Api.Contracts.Agent;
using Microsoft.AspNetCore.Mvc;

namespace ItsmAi.Api.Controllers;

[ApiController]
[Route("api/agent/tools")]
public class AgentToolsController : ControllerBase
{
    private readonly IAgentToolRegistry _toolRegistry;

    public AgentToolsController(
        IAgentToolRegistry toolRegistry)
    {
        _toolRegistry = toolRegistry;
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute(
        ExecuteToolRequest request,
        CancellationToken cancellationToken)
    {
        var tool = _toolRegistry.Get(request.Tool);

        if (tool is null)
        {
            return NotFound(new
            {
                error = $"Tool '{request.Tool}' was not found."
            });
        }

        var result = await tool.ExecuteAsync(
            request.Arguments,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new
            {
                error = result.Error
            });
        }

        return Ok(result.Data);
    }
}