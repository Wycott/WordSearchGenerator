namespace WordSearchGen.Core;

/// <summary>
/// A generated puzzle: the grid and where each word was hidden.
/// </summary>
public sealed class WordSearch(Grid grid, IReadOnlyList<WordPlacement> placements)
{
    public Grid Grid { get; } = grid;

    public IReadOnlyList<WordPlacement> Placements { get; } = placements;
}

/// <summary>
/// Thrown when the words could not all be placed within the allowed number of attempts.
/// </summary>
public sealed class WordSearchGenerationException(string message) : Exception(message);
