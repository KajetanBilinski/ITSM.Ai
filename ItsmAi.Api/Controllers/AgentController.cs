using ItsmAi.Agent.Abstractions.Agent;
using ItsmAi.Api.Contracts.Agent;
using Microsoft.AspNetCore.Mvc;

namespace ItsmAi.Api.Controllers;

[ApiController]
[Route("api/agent")]
public class AgentController : ControllerBase
{
    private readonly IAgentOrchestrator _agentOrchestrator;

    public AgentController(
        IAgentOrchestrator agentOrchestrator)
    {
        _agentOrchestrator = agentOrchestrator;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        AgentChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                error = "Message is required."
            });
        }

        var response =
            await _agentOrchestrator.RunAsync(
                request.Message,
                cancellationToken);

        return Ok(response);
    }
}