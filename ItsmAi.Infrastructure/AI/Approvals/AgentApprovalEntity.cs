namespace ItsmAi.Infrastructure.Ai.Approvals;

public class AgentApprovalEntity
{
    public Guid Id { get; set; }

    public Guid AgentRunId { get; set; }

    public string ToolName { get; set; } =
        string.Empty;

    public string ArgumentsJson { get; set; } =
        string.Empty;

    public string Status { get; set; } =
        string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? DecidedAt { get; set; }
}