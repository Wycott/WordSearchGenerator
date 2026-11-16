using WordSearchGen.Core;

namespace WordSearchGen.Tests;

public class WordSearchTests
{
    [Fact]
    public void Find_ReturnsWhereTheWordWasPlaced()
    {
        var puzzle = new WordSearchGenerator(12, new Random(1)).Generate(WordLists.Heroes);

        foreach (var word in WordLists.Heroes)
        {
            var placement = puzzle.Find(word);

            Assert.NotNull(placement);
            Assert.Equal(word.ToUpperInvariant(), TestHelpers.ReadWord(puzzle.Grid, placement));
        }
    }

    [Fact]
    public void Find_StillWorksAfterTheGridIsFilled()
    {
        var puzzle = new WordSearchGenerator(12, new Random(2)).Generate(WordLists.Food);
        puzzle.Grid.FillEmpty(new Random(3), LetterDistribution.English);

        Assert.All(WordLists.Food, word =>
            Assert.Equal(word.ToUpperInvariant(), TestHelpers.ReadWord(puzzle.Grid, puzzle.Find(word)!)));
    }

    [Fact]
    public void Find_IgnoresCaseAndSurroundingSpaces()
    {
        var puzzle = new WordSearchGenerator(6, new Random(3)).Generate(["cat"]);

        Assert.Same(puzzle.Placements[0], puzzle.Find("  Cat "));
    }

    [Fact]
    public void Find_WordNotInThePuzzle_ReturnsNull()
    {
        var puzzle = new WordSearchGenerator(6, new Random(3)).Generate(["cat"]);

        Assert.Null(puzzle.Find("dog"));
    }

    [Fact]
    public void Find_Null_Throws()
    {
        var puzzle = new WordSearchGenerator(6, new Random(3)).Generate(["cat"]);

        Assert.Throws<ArgumentNullException>(() => puzzle.Find(null!));
    }
}
