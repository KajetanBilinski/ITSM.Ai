namespace ItsmAi.Agent.Abstractions.Agent;

public interface IAgentOrchestrator
{
    Task<AgentResponse> RunAsync(
        string userMessage,
        CancellationToken cancellationToken = default);
}