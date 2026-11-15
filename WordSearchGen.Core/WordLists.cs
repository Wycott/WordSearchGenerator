namespace WordSearchGen.Core;

/// <summary>
/// Word lists for puzzles. Old lists are kept for posterity, testing and recreating old puzzles.
/// </summary>
public static class WordLists
{
    public static IReadOnlyList<string> Insects { get; } =
    [
        "ant", "ladybird", "butterfly", "spider", "centipede",
        "caterpillar", "beetle", "cricket", "woodlouse", "wasp"
    ];

    public static IReadOnlyList<string> Food { get; } =
    [
        "pizza", "chips", "sausage", "beans", "chocolate",
        "crisps", "doughnut", "cake", "milkshake", "haribos"
    ];

    public static IReadOnlyList<string> Heroes { get; } =
    [
        "superman", "starfire", "raven", "penguin", "nightwing",
        "krypto", "joker", "cyborg", "batman", "beastboy"
    ];

    /// <summary>
    /// The list the app uses. Point this at a different list to make a new puzzle.
    /// </summary>
    public static IReadOnlyList<string> Current => Heroes;
}
