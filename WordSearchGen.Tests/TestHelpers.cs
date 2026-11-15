using WordSearchGen.Core;

namespace WordSearchGen.Tests;

internal static class TestHelpers
{
    /// <summary>
    /// Read the letters along a placement's cells.
    /// </summary>
    public static string ReadWord(Grid grid, WordPlacement placement) =>
        new(placement.Cells().Select(c => grid[c.Row, c.Column]).ToArray());

    /// <summary>
    /// Write letters into the grid starting at (row, column) in the given direction,
    /// without the checks <see cref="PlaceFinder.Place"/> makes.
    /// </summary>
    public static void Write(Grid grid, string letters, int row, int column, Orientation orientation)
    {
        var placement = new WordPlacement(letters, row, column, orientation);
        var i = 0;

        foreach (var (r, c) in placement.Cells())
        {
            grid[r, c] = letters[i++];
        }
    }
}
