using WordSearchGen.Core;

namespace WordSearchGen.Tests;

public class PlaceFinderTests
{
    [Fact]
    public void CanPlace_WordThatFitsOnEmptyGrid_IsTrue()
    {
        var grid = new Grid(5);

        Assert.True(PlaceFinder.CanPlace(grid, new WordPlacement("HELLO", 0, 0, Orientation.Right)));
    }

    [Theory]
    [InlineData(0, 1, Orientation.Right)]     // runs off the right edge
    [InlineData(0, 3, Orientation.Left)]      // runs off the left edge
    [InlineData(3, 0, Orientation.Up)]        // runs off the top edge
    [InlineData(1, 0, Orientation.Down)]      // runs off the bottom edge
    [InlineData(1, 1, Orientation.DownRight)]
    [InlineData(3, 3, Orientation.UpLeft)]
    [InlineData(3, 1, Orientation.UpRight)]
    [InlineData(1, 3, Orientation.DownLeft)]
    public void CanPlace_WordRunningOffAnyEdge_IsFalse(int row, int column, Orientation orientation)
    {
        var grid = new Grid(5);

        Assert.False(PlaceFinder.CanPlace(grid, new WordPlacement("HELLO", row, column, orientation)));
    }

    [Fact]
    public void CanPlace_DoesNotWrapAroundToTheNextRow()
    {
        // In the old index-based code, stepping right from the end of a row reached the
        // start of the next row unless an edge check caught it.
        var grid = new Grid(5);

        Assert.False(PlaceFinder.CanPlace(grid, new WordPlacement("AB", 0, 4, Orientation.Right)));
    }

    [Fact]
    public void CanPlace_StartingOutsideTheGrid_IsFalse()
    {
        var grid = new Grid(5);

        Assert.False(PlaceFinder.CanPlace(grid, new WordPlacement("AB", -1, 0, Orientation.Down)));
    }

    [Fact]
    public void CanPlace_CanRunDownThroughTheEndOfTheSecondToLastRow()
    {
        // Regression: AtBottomEdge treated the last cell of the second-to-last row as the bottom row.
        var grid = new Grid(12);

        Assert.True(PlaceFinder.CanPlace(grid, new WordPlacement("AB", 10, 11, Orientation.Down)));
        Assert.True(PlaceFinder.CanPlace(grid, new WordPlacement("AB", 10, 11, Orientation.DownLeft)));
    }

    [Fact]
    public void CanPlace_ReachingExactlyToTheEdge_IsTrue()
    {
        var grid = new Grid(5);

        Assert.True(PlaceFinder.CanPlace(grid, new WordPlacement("HELLO", 4, 4, Orientation.UpLeft)));
        Assert.True(PlaceFinder.CanPlace(grid, new WordPlacement("HELLO", 4, 0, Orientation.UpRight)));
    }

    [Fact]
    public void CanPlace_ChecksTheLastLetter()
    {
        // Regression: the old loop never checked the square under the final letter.
        var grid = new Grid(5);
        grid[0, 4] = 'X';

        Assert.False(PlaceFinder.CanPlace(grid, new WordPlacement("HELLO", 0, 0, Orientation.Right)));
    }

    [Fact]
    public void CanPlace_ChecksTheFirstLetter()
    {
        var grid = new Grid(5);
        grid[0, 0] = 'X';

        Assert.False(PlaceFinder.CanPlace(grid, new WordPlacement("HELLO", 0, 0, Orientation.Right)));
    }

    [Fact]
    public void CanPlace_CrossingOnAMatchingLetter_IsTrue()
    {
        var grid = new Grid(5);
        TestHelpers.Write(grid, "HELLO", 0, 2, Orientation.Down);

        // "BELOW" crosses "HELLO" at the shared L in row 2.
        Assert.True(PlaceFinder.CanPlace(grid, new WordPlacement("BELOW", 2, 0, Orientation.Right)));
    }

    [Fact]
    public void CanPlace_CrossingOnADifferentLetter_IsFalse()
    {
        var grid = new Grid(5);
        TestHelpers.Write(grid, "HELLO", 0, 2, Orientation.Down);

        Assert.False(PlaceFinder.CanPlace(grid, new WordPlacement("BEAST", 2, 0, Orientation.Right)));
    }

    [Fact]
    public void CanPlace_WordEntirelyInsideAnother_IsFalse()
    {
        // Otherwise RAVEN could be "hidden" with every letter already part of BRAVENESS.
        var grid = new Grid(10);
        TestHelpers.Write(grid, "BRAVENESS", 0, 0, Orientation.Right);

        Assert.False(PlaceFinder.CanPlace(grid, new WordPlacement("RAVEN", 0, 1, Orientation.Right)));
    }

    [Fact]
    public void GetOrientationOptions_InCorner_ReturnsOnlyDirectionsAwayFromTheEdges()
    {
        var grid = new Grid(5);

        var options = PlaceFinder.GetOrientationOptions(grid, "HELLO", 0, 0);

        Assert.Equal([Orientation.Down, Orientation.Right, Orientation.DownRight], options);
    }

    [Fact]
    public void GetOrientationOptions_InTheMiddleOfABigGrid_ReturnsAllDirections()
    {
        var grid = new Grid(9);

        var options = PlaceFinder.GetOrientationOptions(grid, "CAT", 4, 4);

        Assert.Equal(Directions.All, options);
    }

    [Fact]
    public void GetOrientationOptions_OnlyReturnsAllowedOrientations()
    {
        var grid = new Grid(9);

        var options = PlaceFinder.GetOrientationOptions(
            grid, "CAT", 4, 4, [Orientation.Left, Orientation.DownRight]);

        Assert.Equal([Orientation.Left, Orientation.DownRight], options);
    }

    [Fact]
    public void GetOrientationOptions_WhenNothingFits_IsEmpty()
    {
        var grid = new Grid(3);

        Assert.Empty(PlaceFinder.GetOrientationOptions(grid, "HELLO", 1, 1));
    }

    [Fact]
    public void Place_WritesEveryLetter()
    {
        var grid = new Grid(5);
        var placement = new WordPlacement("HELLO", 4, 0, Orientation.UpRight);

        PlaceFinder.Place(grid, placement);

        Assert.Equal("HELLO", TestHelpers.ReadWord(grid, placement));
    }

    [Fact]
    public void Place_AllowsCrossingOnAMatchingLetter()
    {
        // Regression: PlaceWord used to throw on any filled square, even a matching one.
        var grid = new Grid(5);
        PlaceFinder.Place(grid, new WordPlacement("HELLO", 0, 2, Orientation.Down));
        var crossing = new WordPlacement("BELOW", 2, 0, Orientation.Right);

        PlaceFinder.Place(grid, crossing);

        Assert.Equal("BELOW", TestHelpers.ReadWord(grid, crossing));
        Assert.Equal("HELLO", TestHelpers.ReadWord(grid, new WordPlacement("HELLO", 0, 2, Orientation.Down)));
    }

    [Fact]
    public void Place_WhenItDoesNotFit_ThrowsAndLeavesTheGridUnchanged()
    {
        var grid = new Grid(5);
        grid[0, 4] = 'X';

        Assert.Throws<InvalidOperationException>(
            () => PlaceFinder.Place(grid, new WordPlacement("HELLO", 0, 0, Orientation.Right)));
        Assert.True(grid.IsEmpty(0, 0));
    }
}
