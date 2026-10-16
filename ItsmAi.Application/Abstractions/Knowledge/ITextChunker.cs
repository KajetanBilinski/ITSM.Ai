namespace ItsmAi.Application.Abstractions.Knowledge;

public interface ITextChunker
{
    IReadOnlyList<string> Chunk(
        string text);
}