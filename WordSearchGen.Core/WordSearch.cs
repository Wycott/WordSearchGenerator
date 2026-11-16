namespace WordSearchGen.Core;

/// <summary>
/// A generated puzzle: the grid and where each word was hidden.
/// </summary>
public sealed class WordSearch(Grid grid, IReadOnlyList<WordPlacement> placements)
{
    public Grid Grid { get; } = grid;

    public IReadOnlyList<WordPlacement> Placements { get; } = placements;

    /// <summary>
    /// Where the given word was hidden, or null if it is not in the puzzle.
    /// Case and surrounding spaces are ignored, as they are when generating.
    /// </summary>
    public WordPlacement? Find(string word)
    {
        ArgumentNullException.ThrowIfNull(word);

        var normalised = word.Trim().ToUpperInvariant();

        return Placements.FirstOrDefault(p => p.Word == normalised);
    }
}

/// <summary>
/// Thrown when the words could not all be placed within the allowed number of attempts.
/// </summary>
public sealed class WordSearchGenerationException(string message) : Exception(message);
