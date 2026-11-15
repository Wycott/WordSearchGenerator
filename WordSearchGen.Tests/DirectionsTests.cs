using WordSearchGen.Core;

namespace WordSearchGen.Tests;

public class DirectionsTests
{
    [Fact]
    public void All_ContainsEveryOrientationOnce()
    {
        Assert.Equal(8, Directions.All.Count);
        Assert.Equal(Enum.GetValues<Orientation>(), Directions.All);
    }

    [Fact]
    public void StepOf_GivesEightDistinctUnitSteps()
    {
        var steps = Directions.All.Select(Directions.StepOf).ToList();

        Assert.Equal(8, steps.Distinct().Count());
        Assert.All(steps, s =>
        {
            Assert.InRange(s.RowStep, -1, 1);
            Assert.InRange(s.ColumnStep, -1, 1);
            Assert.NotEqual((0, 0), s);
        });
    }

    [Theory]
    [InlineData(Orientation.Down, 1, 0)]
    [InlineData(Orientation.Right, 0, 1)]
    [InlineData(Orientation.Up, -1, 0)]
    [InlineData(Orientation.Left, 0, -1)]
    [InlineData(Orientation.UpRight, -1, 1)]
    [InlineData(Orientation.DownRight, 1, 1)]
    [InlineData(Orientation.DownLeft, 1, -1)]
    [InlineData(Orientation.UpLeft, -1, -1)]
    public void StepOf_MatchesOrientationName(Orientation orientation, int rowStep, int columnStep)
    {
        Assert.Equal((rowStep, columnStep), Directions.StepOf(orientation));
    }

    [Fact]
    public void StepOf_UnknownValue_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Directions.StepOf((Orientation)99));
    }
}
