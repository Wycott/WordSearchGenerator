using WordSearchGen.Core;

namespace WordSearchGen.Tests;

public class LetterDistributionTests
{
    private const int Samples = 200_000;

    private static Dictionary<char, double> SampleShares(LetterDistribution distribution, int seed)
    {
        var random = new Random(seed);
        var counts = new Dictionary<char, int>();

        for (var i = 0; i < Samples; i++)
        {
            var letter = distribution.Next(random);
            counts[letter] = counts.GetValueOrDefault(letter) + 1;
        }

        return counts.ToDictionary(c => c.Key, c => (double)c.Value / Samples);
    }

    [Fact]
    public void English_ProbabilitiesAddUpToOne()
    {
        var total = Enumerable.Range('A', 26).Sum(c => LetterDistribution.English.ProbabilityOf((char)c));

        Assert.Equal(1.0, total, precision: 10);
    }

    [Fact]
    public void English_CommonLettersAreMoreLikelyThanRareOnes()
    {
        var english = LetterDistribution.English;

        Assert.True(english.ProbabilityOf('E') > english.ProbabilityOf('T'));
        Assert.True(english.ProbabilityOf('T') > english.ProbabilityOf('Q'));
        Assert.True(english.ProbabilityOf('E') > 100 * english.ProbabilityOf('Z'));
    }

    [Fact]
    public void English_EveryLetterCanStillAppear()
    {
        Assert.All(Enumerable.Range('A', 26), c => Assert.True(LetterDistribution.English.ProbabilityOf((char)c) > 0));
    }

    [Fact]
    public void English_SampledLettersMatchTheTargetFrequencies()
    {
        var shares = SampleShares(LetterDistribution.English, seed: 17);

        Assert.Equal(26, shares.Count);
        foreach (var (letter, share) in shares)
        {
            // About 4 standard deviations for the most common letter at this sample size.
            Assert.InRange(share, LetterDistribution.English.ProbabilityOf(letter) - 0.003,
                LetterDistribution.English.ProbabilityOf(letter) + 0.003);
        }

        Assert.Equal('E', shares.MaxBy(s => s.Value).Key);
    }

    [Fact]
    public void Uniform_SampledLettersAreRoughlyEqual()
    {
        var shares = SampleShares(LetterDistribution.Uniform, seed: 23);

        Assert.Equal(26, shares.Count);
        Assert.All(shares.Values, s => Assert.InRange(s, 1.0 / 26 - 0.003, 1.0 / 26 + 0.003));
    }

    [Fact]
    public void Next_NeverPicksALetterWithZeroWeight()
    {
        var distribution = new LetterDistribution(new Dictionary<char, double> { ['A'] = 1, ['B'] = 0, ['C'] = 3 });

        var shares = SampleShares(distribution, seed: 5);

        Assert.Equal(['A', 'C'], shares.Keys.Order());
        Assert.InRange(shares['C'], 0.74, 0.76);
        Assert.Equal(0, distribution.ProbabilityOf('B'));
    }

    [Fact]
    public void Next_HandlesBoundaryValuesFromTheRandomSource()
    {
        var distribution = new LetterDistribution(new Dictionary<char, double> { ['A'] = 1, ['B'] = 1 });

        Assert.Equal('A', distribution.Next(new FixedRandom(0.0)));
        Assert.Equal('B', distribution.Next(new FixedRandom(0.5)));
        Assert.Equal('B', distribution.Next(new FixedRandom(0.9999999999)));
    }

    [Fact]
    public void Next_SingleLetter_AlwaysReturnsIt()
    {
        var distribution = new LetterDistribution(new Dictionary<char, double> { ['Q'] = 2 });

        Assert.All(Enumerable.Range(0, 20), _ => Assert.Equal('Q', distribution.Next(new Random(1))));
    }

    [Theory]
    [InlineData('a', 1.0)]
    [InlineData('1', 1.0)]
    [InlineData('A', -1.0)]
    [InlineData('A', double.NaN)]
    [InlineData('A', double.PositiveInfinity)]
    public void Constructor_RejectsBadWeights(char letter, double weight)
    {
        Assert.Throws<ArgumentException>(
            () => new LetterDistribution(new Dictionary<char, double> { [letter] = weight, ['B'] = 1 }));
    }

    [Fact]
    public void Constructor_RejectsAllZeroWeights()
    {
        Assert.Throws<ArgumentException>(
            () => new LetterDistribution(new Dictionary<char, double> { ['A'] = 0, ['B'] = 0 }));
    }

    [Fact]
    public void Constructor_RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new LetterDistribution(null!));
    }

    private sealed class FixedRandom(double value) : Random
    {
        public override double NextDouble() => value;
    }
}
