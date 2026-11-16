using System.Diagnostics;
using WordSearchGen.Core;

namespace WordSearchGen.Tests;

public class WordSearchGeneratorTests
{
    [Fact]
    public void Generate_PlacesEveryWordWhereItSaysItDid()
    {
        var generator = new WordSearchGenerator(12, new Random(1));

        var puzzle = generator.Generate(WordLists.Heroes);

        Assert.Equal(WordLists.Heroes.Count, puzzle.Placements.Count);
        Assert.Equal(
            WordLists.Heroes.Select(w => w.ToUpperInvariant()).Order(),
            puzzle.Placements.Select(p => p.Word).Order());
        Assert.All(puzzle.Placements, p => Assert.Equal(p.Word, TestHelpers.ReadWord(puzzle.Grid, p)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Generate_EveryWordListFitsInTheAppsGrid(int seed)
    {
        var generator = new WordSearchGenerator(12, new Random(seed));

        foreach (var list in new[] { WordLists.Insects, WordLists.Food, WordLists.Heroes })
        {
            var puzzle = generator.Generate(list);

            Assert.All(puzzle.Placements, p => Assert.Equal(p.Word, TestHelpers.ReadWord(puzzle.Grid, p)));
        }
    }

    [Fact]
    public void Generate_KeepsEveryPlacementInsideTheGrid()
    {
        var puzzle = new WordSearchGenerator(12, new Random(5)).Generate(WordLists.Insects);

        Assert.All(
            puzzle.Placements.SelectMany(p => p.Cells()),
            c => Assert.True(puzzle.Grid.IsInBounds(c.Row, c.Column)));
    }

    [Fact]
    public void Generate_LeavesUnusedCellsEmpty()
    {
        var puzzle = new WordSearchGenerator(12, new Random(2)).Generate(["cat"]);

        var filled = 0;
        for (var r = 0; r < 12; r++)
        {
            for (var c = 0; c < 12; c++)
            {
                if (!puzzle.Grid.IsEmpty(r, c))
                {
                    filled++;
                }
            }
        }

        Assert.Equal(3, filled);
    }

    [Fact]
    public void Generate_NormalisesWordsToUpperCase()
    {
        var puzzle = new WordSearchGenerator(6, new Random(3)).Generate([" Cat "]);

        Assert.Equal("CAT", Assert.Single(puzzle.Placements).Word);
    }

    [Fact]
    public void Generate_IsRepeatableWithTheSameSeed()
    {
        var first = new WordSearchGenerator(12, new Random(123)).Generate(WordLists.Food);
        var second = new WordSearchGenerator(12, new Random(123)).Generate(WordLists.Food);

        Assert.Equal(first.Placements, second.Placements);
    }

    [Fact]
    public void Generate_WordLongerThanTheGrid_ThrowsArgumentException()
    {
        // Previously this spun forever on the UI thread.
        var generator = new WordSearchGenerator(5, new Random(1));

        var ex = Assert.Throws<ArgumentException>(() => generator.Generate(["caterpillar"]));
        Assert.Contains("caterpillar", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("bat-man")]
    [InlineData("r2d2")]
    public void Generate_InvalidWord_ThrowsArgumentException(string? word)
    {
        var generator = new WordSearchGenerator(12, new Random(1));

        Assert.Throws<ArgumentException>(() => generator.Generate([word!]));
    }

    [Fact]
    public void Generate_NullWordList_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new WordSearchGenerator(5).Generate(null!));
    }

    [Fact]
    public void Generate_WhenWordsCannotAllFit_GivesUpWithAnException()
    {
        // Three two-letter words with no shared letters need six cells; a 2x2 grid has four.
        var generator = new WordSearchGenerator(2, new Random(1), maxAttemptsPerWord: 50, maxBoardAttempts: 20);
        var timer = Stopwatch.StartNew();

        Assert.Throws<WordSearchGenerationException>(() => generator.Generate(["ab", "cd", "ef"]));
        Assert.True(timer.Elapsed < TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Generate_CanCrossWordsToFitThemIn()
    {
        // Four 3-letter words in 9 cells only fit if all four cross on their shared middle B.
        var generator = new WordSearchGenerator(3, new Random(4));

        var puzzle = generator.Generate(["abc", "dbe", "fbg", "hbi"]);

        Assert.All(puzzle.Placements, p => Assert.Equal(p.Word, TestHelpers.ReadWord(puzzle.Grid, p)));
        Assert.Equal('B', puzzle.Grid[1, 1]);
    }

    [Fact]
    public void Generate_EmptyWordList_ReturnsAnEmptyGrid()
    {
        var puzzle = new WordSearchGenerator(4).Generate([]);

        Assert.Empty(puzzle.Placements);
        Assert.Equal(4, puzzle.Grid.Size);
    }

    [Theory]
    [InlineData(0, 10, 10)]
    [InlineData(5, 0, 10)]
    [InlineData(5, 10, 0)]
    public void Constructor_RejectsNonPositiveSettings(int size, int perWord, int perBoard)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WordSearchGenerator(size, null, perWord, perBoard));
    }

    [Fact]
    public void ChooseOrientation_PicksOnlyFromTheLeastUsedOptions()
    {
        var usage = Directions.All.ToDictionary(o => o, _ => 3);
        usage[Orientation.Left] = 1;
        usage[Orientation.UpLeft] = 1;
        usage[Orientation.Down] = 0; // least used overall, but not one of the options
        var options = new[] { Orientation.Right, Orientation.Left, Orientation.UpLeft };
        var random = new Random(8);

        var chosen = Enumerable.Range(0, 50)
            .Select(_ => WordSearchGenerator.ChooseOrientation(options, usage, random))
            .ToHashSet();

        Assert.Equal([Orientation.Left, Orientation.UpLeft], chosen.Order());
    }

    [Fact]
    public void ChooseOrientation_DoesNotAlwaysPickTheFirstOption()
    {
        // The old code fell back to options[0], which favoured Down.
        var usage = Directions.All.ToDictionary(o => o, _ => 0);
        var random = new Random(11);

        var chosen = Enumerable.Range(0, 100)
            .Select(_ => WordSearchGenerator.ChooseOrientation(Directions.All, usage, random))
            .ToHashSet();

        Assert.Equal(8, chosen.Count);
    }

    [Fact]
    public void Generate_UsesEveryDirection()
    {
        var words = Enumerable.Range(0, 16).Select(i => new string((char)('A' + i), 2));
        var puzzle = new WordSearchGenerator(20, new Random(6)).Generate(words);

        Assert.Equal(8, puzzle.Placements.Select(p => p.Orientation).Distinct().Count());
    }

    [Theory]
    [InlineData(SkillLevel.Beginner)]
    [InlineData(SkillLevel.Intermediate)]
    [InlineData(SkillLevel.Advanced)]
    public void Generate_OnlyUsesTheSkillLevelsDirections(SkillLevel level)
    {
        var allowed = Directions.For(level);

        for (var seed = 1; seed <= 5; seed++)
        {
            var puzzle = new WordSearchGenerator(12, new Random(seed)).Generate(WordLists.Heroes, level);

            Assert.All(puzzle.Placements, p => Assert.Contains(p.Orientation, allowed));
            Assert.All(puzzle.Placements, p => Assert.Equal(p.Word, TestHelpers.ReadWord(puzzle.Grid, p)));
        }
    }

    [Theory]
    [InlineData(SkillLevel.Beginner, 2)]
    [InlineData(SkillLevel.Intermediate, 4)]
    [InlineData(SkillLevel.Advanced, 8)]
    public void Generate_SpreadsWordsAcrossEveryAllowedDirection(SkillLevel level, int expected)
    {
        var words = Enumerable.Range(0, 16).Select(i => new string((char)('A' + i), 2));
        var puzzle = new WordSearchGenerator(20, new Random(6)).Generate(words, level);

        Assert.Equal(expected, puzzle.Placements.Select(p => p.Orientation).Distinct().Count());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Generate_EveryWordListFitsAtBeginnerLevel(int seed)
    {
        // Beginner is the most constrained level, so check the real lists still fit.
        var generator = new WordSearchGenerator(12, new Random(seed));

        foreach (var list in new[] { WordLists.Insects, WordLists.Food, WordLists.Heroes })
        {
            var puzzle = generator.Generate(list, SkillLevel.Beginner);

            Assert.Equal(list.Count, puzzle.Placements.Count);
        }
    }

    [Fact]
    public void Generate_UnknownSkillLevel_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WordSearchGenerator(5).Generate(["cat"], (SkillLevel)99));
    }
}
