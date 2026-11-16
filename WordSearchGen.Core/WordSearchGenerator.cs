namespace WordSearchGen.Core;

/// <summary>
/// Hides a list of words in a square grid.
/// </summary>
public sealed class WordSearchGenerator
{
    public const int DefaultMaxAttemptsPerWord = 1000;
    public const int DefaultMaxBoardAttempts = 200;

    private readonly Random random;

    public WordSearchGenerator(
        int gridSize,
        Random? random = null,
        int maxAttemptsPerWord = DefaultMaxAttemptsPerWord,
        int maxBoardAttempts = DefaultMaxBoardAttempts)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(gridSize, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxAttemptsPerWord, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxBoardAttempts, 1);

        GridSize = gridSize;
        this.random = random ?? Random.Shared;
        MaxAttemptsPerWord = maxAttemptsPerWord;
        MaxBoardAttempts = maxBoardAttempts;
    }

    public int GridSize { get; }

    public int MaxAttemptsPerWord { get; }

    public int MaxBoardAttempts { get; }

    /// <summary>
    /// Build a grid containing every word. Unused cells are left empty; call
    /// <see cref="Grid.FillEmpty(Random)"/> to fill them with uniformly random letters, or
    /// <see cref="Grid.FillEmpty(Random, LetterDistribution)"/> to weight them, for example
    /// with <see cref="LetterDistribution.English"/>.
    /// </summary>
    /// <param name="words">The words to hide.</param>
    /// <param name="level">Decides which directions words may run in; see <see cref="Directions.For"/>.</param>
    /// <exception cref="ArgumentException">A word is empty, contains non-letters, or is longer than the grid.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="level"/> is not a defined skill level.</exception>
    /// <exception cref="WordSearchGenerationException">The words could not all be fitted in.</exception>
    public WordSearch Generate(IEnumerable<string> words, SkillLevel level = SkillLevel.Advanced)
    {
        var allowed = Directions.For(level);
        var normalised = NormaliseWords(words);

        // Longest words are the hardest to fit, so place them first.
        var placementOrder = normalised.OrderByDescending(w => w.Length).ToList();

        for (var attempt = 0; attempt < MaxBoardAttempts; attempt++)
        {
            var result = TryBuildBoard(placementOrder, allowed);

            if (result != null)
            {
                return result;
            }
        }

        throw new WordSearchGenerationException(
            $"Could not fit all {normalised.Count} words into a {GridSize}x{GridSize} grid after {MaxBoardAttempts} attempts.");
    }

    private List<string> NormaliseWords(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        var result = new List<string>();

        foreach (var raw in words)
        {
            // Callers can still pass null despite the annotation, so check before trimming.
            if (string.IsNullOrWhiteSpace(raw))
            {
                throw new ArgumentException("Words must not be empty.", nameof(words));
            }

            var word = raw.Trim().ToUpperInvariant();

            if (!word.All(c => c is >= 'A' and <= 'Z'))
            {
                throw new ArgumentException($"'{raw}' must contain only the letters A-Z.", nameof(words));
            }

            if (word.Length > GridSize)
            {
                throw new ArgumentException(
                    $"'{raw}' has {word.Length} letters and cannot fit in a {GridSize}x{GridSize} grid.", nameof(words));
            }

            result.Add(word);
        }

        return result;
    }

    private WordSearch? TryBuildBoard(IReadOnlyList<string> words, IReadOnlyList<Orientation> allowed)
    {
        var grid = new Grid(GridSize);
        var placements = new List<WordPlacement>();
        var usage = allowed.ToDictionary(o => o, _ => 0);

        foreach (var word in words)
        {
            var placement = TryPlaceWord(grid, word, allowed, usage);

            if (placement == null)
            {
                return null;
            }

            PlaceFinder.Place(grid, placement);
            usage[placement.Orientation]++;
            placements.Add(placement);
        }

        return new WordSearch(grid, placements);
    }

    private WordPlacement? TryPlaceWord(
        Grid grid,
        string word,
        IReadOnlyList<Orientation> allowed,
        IReadOnlyDictionary<Orientation, int> usage)
    {
        for (var attempt = 0; attempt < MaxAttemptsPerWord; attempt++)
        {
            var row = random.Next(GridSize);
            var column = random.Next(GridSize);
            var options = PlaceFinder.GetOrientationOptions(grid, word, row, column, allowed);

            if (options.Count > 0)
            {
                return new WordPlacement(word, row, column, ChooseOrientation(options, usage, random));
            }
        }

        return null;
    }

    /// <summary>
    /// Pick one of the options that has been used least so far, so words are spread
    /// across all directions. Ties are broken at random.
    /// </summary>
    internal static Orientation ChooseOrientation(
        IReadOnlyList<Orientation> options,
        IReadOnlyDictionary<Orientation, int> usage,
        Random random)
    {
        var fewest = options.Min(o => usage[o]);
        var leastUsed = options.Where(o => usage[o] == fewest).ToList();

        return leastUsed[random.Next(leastUsed.Count)];
    }
}
