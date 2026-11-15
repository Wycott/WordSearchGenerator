namespace WordSearchGen.Core;

/// <summary>
/// How likely each letter is to be picked when filling empty squares.
/// </summary>
public sealed class LetterDistribution
{
    private readonly char[] letters;
    private readonly double[] cumulativeWeights;

    /// <param name="weights">
    /// Relative weight of each capital letter. Weights need not add up to anything in
    /// particular; letters left out (or given 0) are never picked.
    /// </param>
    public LetterDistribution(IReadOnlyDictionary<char, double> weights)
    {
        ArgumentNullException.ThrowIfNull(weights);

        var entries = weights.Where(w => w.Value > 0).OrderBy(w => w.Key).ToList();

        foreach (var (letter, weight) in weights)
        {
            if (letter is < 'A' or > 'Z')
            {
                throw new ArgumentException($"'{letter}' is not a capital letter A-Z.", nameof(weights));
            }

            if (weight < 0 || !double.IsFinite(weight))
            {
                throw new ArgumentException($"The weight for '{letter}' must be a finite number of 0 or more.", nameof(weights));
            }
        }

        if (entries.Count == 0)
        {
            throw new ArgumentException("At least one letter must have a weight above 0.", nameof(weights));
        }

        letters = entries.Select(e => e.Key).ToArray();
        cumulativeWeights = new double[entries.Count];

        var total = 0.0;
        for (var i = 0; i < entries.Count; i++)
        {
            total += entries[i].Value;
            cumulativeWeights[i] = total;
        }
    }

    /// <summary>
    /// Every letter is equally likely.
    /// </summary>
    public static LetterDistribution Uniform { get; } =
        new(Enumerable.Range('A', 26).ToDictionary(c => (char)c, _ => 1.0));

    /// <summary>
    /// Letters appear about as often as they do in English text, so E, T and A are common
    /// and Q, X and Z are rare. Figures are percentages of letters in typical English.
    /// </summary>
    public static LetterDistribution English { get; } = new(new Dictionary<char, double>
    {
        ['A'] = 8.2, ['B'] = 1.5, ['C'] = 2.8, ['D'] = 4.3, ['E'] = 12.7, ['F'] = 2.2,
        ['G'] = 2.0, ['H'] = 6.1, ['I'] = 7.0, ['J'] = 0.15, ['K'] = 0.77, ['L'] = 4.0,
        ['M'] = 2.4, ['N'] = 6.7, ['O'] = 7.5, ['P'] = 1.9, ['Q'] = 0.095, ['R'] = 6.0,
        ['S'] = 6.3, ['T'] = 9.1, ['U'] = 2.8, ['V'] = 0.98, ['W'] = 2.4, ['X'] = 0.15,
        ['Y'] = 2.0, ['Z'] = 0.074
    });

    /// <summary>
    /// The chance (between 0 and 1) that <see cref="Next"/> returns the given letter.
    /// </summary>
    public double ProbabilityOf(char letter)
    {
        var i = Array.IndexOf(letters, letter);

        if (i < 0)
        {
            return 0;
        }

        var weight = cumulativeWeights[i] - (i == 0 ? 0 : cumulativeWeights[i - 1]);
        return weight / cumulativeWeights[^1];
    }

    /// <summary>
    /// Pick a letter at random according to the weights.
    /// </summary>
    public char Next(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);

        var target = random.NextDouble() * cumulativeWeights[^1];
        var i = Array.BinarySearch(cumulativeWeights, target);

        // BinarySearch returns the bitwise complement of the next larger entry when there is
        // no exact match. An exact match sits on a boundary and belongs to the next letter.
        i = i < 0 ? ~i : i + 1;

        return letters[Math.Min(i, letters.Length - 1)];
    }
}
