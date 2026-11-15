namespace WordSearchGen.Core;

/// <summary>
/// A square grid of letters. Empty cells hold <see cref="Empty"/>.
/// </summary>
public sealed class Grid
{
    public const char Empty = '\0';

    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private readonly char[,] cells;

    public Grid(int size)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        Size = size;
        cells = new char[size, size];
    }

    /// <summary>
    /// Length (in letters) of each side of the grid.
    /// </summary>
    public int Size { get; }

    public char this[int row, int column]
    {
        get => cells[row, column];
        set => cells[row, column] = value;
    }

    public bool IsInBounds(int row, int column) =>
        row >= 0 && row < Size && column >= 0 && column < Size;

    public bool IsEmpty(int row, int column) => cells[row, column] == Empty;

    public void Clear() => Array.Clear(cells);

    /// <summary>
    /// Put a random letter in every empty cell, leaving placed words untouched.
    /// </summary>
    public void FillEmpty(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        for (var row = 0; row < Size; row++)
        {
            for (var column = 0; column < Size; column++)
            {
                if (IsEmpty(row, column))
                {
                    cells[row, column] = Alphabet[random.Next(Alphabet.Length)];
                }
            }
        }
    }
}
