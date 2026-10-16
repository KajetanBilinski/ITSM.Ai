namespace ItsmAi.Application.Abstractions.Ai;

public interface IEmbeddingProvider
{
    Task<float[]> GenerateAsync(
        string text,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<float[]>> GenerateBatchAsync(
        IReadOnlyList<string> texts,
        CancellationToken cancellationToken = default);
}