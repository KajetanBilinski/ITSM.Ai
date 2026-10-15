using ItsmAi.Agent.Abstractions.Approvals;
using Microsoft.AspNetCore.Mvc;

namespace ItsmAi.Api.Controllers;

[ApiController]
[Route("api/agent/approvals")]
public class AgentApprovalsController
    : ControllerBase
{
    private readonly IAgentApprovalService _approvalService;

    public AgentApprovalsController(
        IAgentApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    [HttpGet("{approvalId:guid}")]
    public async Task<IActionResult> Get(
        Guid approvalId,
        CancellationToken cancellationToken)
    {
        var approval =
            await _approvalService.GetAsync(
                approvalId,
                cancellationToken);

        if (approval is null)
        {
            return NotFound();
        }

        return Ok(approval);
    }

    [HttpPost("{approvalId:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid approvalId,
        CancellationToken cancellationToken)
    {
        var result =
            await _approvalService.ApproveAsync(
                approvalId,
                cancellationToken);

        return result.Status switch
        {
            AgentApprovalDecisionStatus.Approved =>
                Ok(result),

            AgentApprovalDecisionStatus.NotFound =>
                NotFound(result),

            AgentApprovalDecisionStatus.NotPending =>
                Conflict(result),

            AgentApprovalDecisionStatus.Failed =>
                BadRequest(result),

            _ =>
                StatusCode(
                    StatusCodes.Status500InternalServerError,
                    result)
        };
    }

    [HttpPost("{approvalId:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid approvalId,
        CancellationToken cancellationToken)
    {
        var result =
            await _approvalService.RejectAsync(
                approvalId,
                cancellationToken);

        return result.Status switch
        {
            AgentApprovalDecisionStatus.Rejected =>
                Ok(result),

            AgentApprovalDecisionStatus.NotFound =>
                NotFound(result),

            AgentApprovalDecisionStatus.NotPending =>
                Conflict(result),

            _ =>
                StatusCode(
                    StatusCodes.Status500InternalServerError,
                    result)
        };
    }
}