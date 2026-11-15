namespace WordSearchGen.Core;

/// <summary>
/// Decides whether a word fits on the grid, and writes it there.
/// </summary>
public static class PlaceFinder
{
    /// <summary>
    /// True when every letter of the word lands inside the grid on a cell that is either
    /// empty or already holds that same letter, and at least one cell is empty (so a word
    /// is never hidden entirely inside another one).
    /// </summary>
    public static bool CanPlace(Grid grid, WordPlacement placement)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(placement);

        var usesEmptyCell = false;
        var index = 0;

        foreach (var (row, column) in placement.Cells())
        {
            if (!grid.IsInBounds(row, column))
            {
                return false;
            }

            var existing = grid[row, column];

            if (existing == Grid.Empty)
            {
                usesEmptyCell = true;
            }
            else if (existing != placement.Word[index])
            {
                return false;
            }

            index++;
        }

        return usesEmptyCell;
    }

    /// <summary>
    /// All orientations in which the word fits when it starts at the given cell.
    /// </summary>
    public static List<Orientation> GetOrientationOptions(Grid grid, string word, int row, int column)
    {
        ArgumentNullException.ThrowIfNull(word);

        return Directions.All
            .Where(o => CanPlace(grid, new WordPlacement(word, row, column, o)))
            .ToList();
    }

    /// <summary>
    /// Write the word onto the grid.
    /// </summary>
    /// <exception cref="InvalidOperationException">The word does not fit there.</exception>
    public static void Place(Grid grid, WordPlacement placement)
    {
        if (!CanPlace(grid, placement))
        {
            throw new InvalidOperationException(
                $"'{placement.Word}' does not fit at ({placement.Row}, {placement.Column}) going {placement.Orientation}.");
        }

        var index = 0;

        foreach (var (row, column) in placement.Cells())
        {
            grid[row, column] = placement.Word[index++];
        }
    }
}
