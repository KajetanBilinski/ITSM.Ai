using System.Net.Http.Json;
using System.Text.Json.Serialization;
using ItsmAi.Application.Abstractions.Ai;
using Microsoft.Extensions.Configuration;

namespace ItsmAi.Infrastructure.AI.Ollama;

public class OllamaEmbeddingProvider
    : IEmbeddingProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly int _dimensions;

    public OllamaEmbeddingProvider(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _model = configuration["Embeddings:Model"]
            ?? throw new InvalidOperationException(
                "Embedding model is not configured.");

        if (!int.TryParse(
                configuration["Embeddings:Dimensions"],
                out _dimensions))
        {
            throw new InvalidOperationException(
                "Embedding dimensions are not configured.");
        }
    }

    public async Task<float[]> GenerateAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be empty.",
                nameof(text));
        }

        var embeddings =
            await GenerateInternalAsync(
                text,
                cancellationToken);

        if (embeddings.Count != 1)
        {
            throw new InvalidOperationException(
                "Ollama returned an unexpected number of embeddings.");
        }

        return embeddings[0];
    }

    public async Task<IReadOnlyList<float[]>> GenerateBatchAsync(
        IReadOnlyList<string> texts,
        CancellationToken cancellationToken = default)
    {
        if (texts.Count == 0)
        {
            return Array.Empty<float[]>();
        }

        if (texts.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "Embedding input cannot contain empty text.",
                nameof(texts));
        }

        var embeddings =
            await GenerateInternalAsync(
                texts,
                cancellationToken);

        if (embeddings.Count != texts.Count)
        {
            throw new InvalidOperationException(
                "Ollama returned an unexpected number of embeddings.");
        }

        return embeddings;
    }

    private async Task<IReadOnlyList<float[]>> GenerateInternalAsync(
        object input,
        CancellationToken cancellationToken)
    {
        var request =
            new OllamaEmbedRequest(
                Model: _model,
                Input: input);

        using var response =
            await _httpClient.PostAsJsonAsync(
                "/api/embed",
                request,
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            throw new HttpRequestException(
                $"Ollama embedding request failed with status " +
                $"{(int)response.StatusCode} ({response.StatusCode}). " +
                $"Response: {errorContent}");
        }

        var result =
            await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>(
                cancellationToken: cancellationToken);

        if (result?.Embeddings is null)
        {
            throw new InvalidOperationException(
                "Ollama returned an invalid embedding response.");
        }

        foreach (var embedding in result.Embeddings)
        {
            if (embedding.Length != _dimensions)
            {
                throw new InvalidOperationException(
                    $"Expected embedding dimension {_dimensions}, " +
                    $"but Ollama returned {embedding.Length}.");
            }
        }

        return result.Embeddings;
    }

    private sealed record OllamaEmbedRequest(
        [property: JsonPropertyName("model")]
        string Model,

        [property: JsonPropertyName("input")]
        object Input);

    private sealed record OllamaEmbedResponse(
        [property: JsonPropertyName("embeddings")]
        float[][] Embeddings);
}