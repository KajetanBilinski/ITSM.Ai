using ItsmAi.Application.Abstractions.Knowledge;

namespace ItsmAi.Application.Knowledge.Chunking;

public class WordTextChunker : ITextChunker
{
    private const int MaxWords = 180;
    private const int OverlapWords = 30;

    public IReadOnlyList<string> Chunk(
        string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<string>();
        }

        var words = text.Split(
            (char[]?)null,
            StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
        {
            return Array.Empty<string>();
        }

        var chunks = new List<string>();

        var step =
            MaxWords - OverlapWords;

        for (var start = 0;
             start < words.Length;
             start += step)
        {
            var length = Math.Min(
                MaxWords,
                words.Length - start);

            var chunk = string.Join(
                " ",
                words.Skip(start).Take(length));

            chunks.Add(chunk);

            if (start + length >= words.Length)
            {
                break;
            }
        }

        return chunks;
    }
}