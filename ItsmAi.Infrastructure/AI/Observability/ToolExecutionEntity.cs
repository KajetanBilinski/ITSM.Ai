namespace ItsmAi.Infrastructure.AI.Observability;

public class ToolExecutionEntity
{
    public Guid Id { get; set; }

    public Guid AgentRunId { get; set; }

    public string ToolName { get; set; } = string.Empty;

    public string ArgumentsJson { get; set; } = string.Empty;

    public string ResultJson { get; set; } = string.Empty;

    public bool Success { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime CompletedAt { get; set; }

    public AgentRunEntity AgentRun { get; set; } = null!;
}