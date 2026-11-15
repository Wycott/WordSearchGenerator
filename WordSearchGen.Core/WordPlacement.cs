namespace WordSearchGen.Core;

/// <summary>
/// Where a word sits on the grid: its first letter's cell and the direction it runs in.
/// </summary>
public sealed record WordPlacement(string Word, int Row, int Column, Orientation Orientation)
{
    /// <summary>
    /// The cells the word covers, from its first letter to its last.
    /// </summary>
    public IEnumerable<(int Row, int Column)> Cells()
    {
        var (rowStep, columnStep) = Directions.StepOf(Orientation);

        for (var i = 0; i < Word.Length; i++)
        {
            yield return (Row + i * rowStep, Column + i * columnStep);
        }
    }
}
