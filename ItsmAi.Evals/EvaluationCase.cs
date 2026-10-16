namespace ItsmAi.Evals;

public record EvaluationCase(
    string Name,
    string UserMessage,
    string? ExpectedTool,
    Guid? ExpectedIncidentId = null);