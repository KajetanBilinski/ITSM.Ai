namespace ItsmAi.Infrastructure.AI.Observability;

public class AgentRunEntity
{
    public Guid Id { get; set; }

    public string UserMessage { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? FinalResponse { get; set; }

    public string? Error { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public List<ToolExecutionEntity> ToolExecutions { get; set; } = [];
}