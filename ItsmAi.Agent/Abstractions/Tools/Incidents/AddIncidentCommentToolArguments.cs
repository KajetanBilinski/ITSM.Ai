namespace ItsmAi.Agent.Tools.Incidents;

public record AddIncidentCommentToolArguments(
    Guid IncidentId,
    string Content);