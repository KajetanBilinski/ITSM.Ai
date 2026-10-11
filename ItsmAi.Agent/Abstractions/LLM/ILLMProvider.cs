namespace ItsmAi.Agent.Abstractions.Llm;

public interface ILlmProvider
{
    Task<LlmResponse> CompleteAsync(
        LlmRequest request,
        CancellationToken cancellationToken = default);
}