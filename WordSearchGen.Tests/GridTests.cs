using WordSearchGen.Core;

namespace WordSearchGen.Tests;

public class GridTests
{
    [Fact]
    public void NewGrid_IsEmpty()
    {
        var grid = new Grid(4);

        Assert.Equal(4, grid.Size);
        for (var r = 0; r < 4; r++)
        {
            for (var c = 0; c < 4; c++)
            {
                Assert.True(grid.IsEmpty(r, c));
            }
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Constructor_RejectsNonPositiveSize(int size)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Grid(size));
    }

    [Theory]
    [InlineData(0, 0, true)]
    [InlineData(4, 4, true)]
    [InlineData(-1, 0, false)]
    [InlineData(0, -1, false)]
    [InlineData(5, 0, false)]
    [InlineData(0, 5, false)]
    public void IsInBounds_ChecksAllFourEdges(int row, int column, bool expected)
    {
        Assert.Equal(expected, new Grid(5).IsInBounds(row, column));
    }

    [Fact]
    public void Clear_EmptiesEveryCell()
    {
        var grid = new Grid(3);
        grid.FillEmpty(new Random(1));

        grid.Clear();

        Assert.True(grid.IsEmpty(0, 0));
        Assert.True(grid.IsEmpty(2, 2));
    }

    [Fact]
    public void FillEmpty_FillsEveryCellWithACapitalLetter()
    {
        var grid = new Grid(6);

        grid.FillEmpty(new Random(7));

        for (var r = 0; r < 6; r++)
        {
            for (var c = 0; c < 6; c++)
            {
                Assert.InRange(grid[r, c], 'A', 'Z');
            }
        }
    }

    [Fact]
    public void FillEmpty_LeavesExistingLettersAlone()
    {
        var grid = new Grid(5);
        TestHelpers.Write(grid, "HELLO", 2, 0, Orientation.Right);

        grid.FillEmpty(new Random(3));

        Assert.Equal("HELLO", TestHelpers.ReadWord(grid, new WordPlacement("HELLO", 2, 0, Orientation.Right)));
    }

    [Fact]
    public void FillEmpty_UsesTheWholeAlphabet()
    {
        // The old GUID-based picker favoured A-L; a fair picker reaches every letter.
        var grid = new Grid(40);

        grid.FillEmpty(new Random(42));

        var letters = new HashSet<char>();
        for (var r = 0; r < 40; r++)
        {
            for (var c = 0; c < 40; c++)
            {
                letters.Add(grid[r, c]);
            }
        }

        Assert.Equal(26, letters.Count);
    }

    [Fact]
    public void FillEmpty_WithDistribution_UsesIt()
    {
        var grid = new Grid(4);
        TestHelpers.Write(grid, "CAT", 0, 0, Orientation.Right);
        var onlyQ = new LetterDistribution(new Dictionary<char, double> { ['Q'] = 1 });

        grid.FillEmpty(new Random(1), onlyQ);

        Assert.Equal("CAT", TestHelpers.ReadWord(grid, new WordPlacement("CAT", 0, 0, Orientation.Right)));
        Assert.Equal('Q', grid[0, 3]);
        Assert.Equal('Q', grid[3, 3]);
    }

    [Fact]
    public void FillEmpty_WithEnglishDistribution_HasMoreEsThanZs()
    {
        var grid = new Grid(60);

        grid.FillEmpty(new Random(4), LetterDistribution.English);

        var es = 0;
        var zs = 0;
        for (var r = 0; r < 60; r++)
        {
            for (var c = 0; c < 60; c++)
            {
                es += grid[r, c] == 'E' ? 1 : 0;
                zs += grid[r, c] == 'Z' ? 1 : 0;
            }
        }

        Assert.True(es > 20 * Math.Max(zs, 1), $"E appeared {es} times and Z {zs} times.");
    }

    [Fact]
    public void FillEmpty_NullDistribution_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new Grid(3).FillEmpty(new Random(1), null!));
    }

    [Fact]
    public void FillEmpty_IsRepeatableWithTheSameSeed()
    {
        var first = new Grid(8);
        var second = new Grid(8);

        first.FillEmpty(new Random(99));
        second.FillEmpty(new Random(99));

        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                Assert.Equal(first[r, c], second[r, c]);
            }
        }
    }
}
