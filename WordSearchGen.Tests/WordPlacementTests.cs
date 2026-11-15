using WordSearchGen.Core;

namespace WordSearchGen.Tests;

public class WordPlacementTests
{
    [Fact]
    public void Cells_StartAtFirstLetterAndFollowTheDirection()
    {
        var placement = new WordPlacement("CAT", 5, 5, Orientation.UpLeft);

        Assert.Equal([(5, 5), (4, 4), (3, 3)], placement.Cells());
    }

    [Fact]
    public void Cells_HasOneCellPerLetter()
    {
        var placement = new WordPlacement("NIGHTWING", 0, 0, Orientation.Down);

        Assert.Equal(9, placement.Cells().Count());
    }
}
