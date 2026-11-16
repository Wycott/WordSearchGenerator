namespace WordSearchGen.Core;

/// <summary>
/// How hard the puzzle is, which decides the directions words may run in.
/// </summary>
public enum SkillLevel
{
    /// <summary>
    /// Words run across (left to right) or down only.
    /// </summary>
    Beginner,

    /// <summary>
    /// Words run across or down, forwards or backwards.
    /// </summary>
    Intermediate,

    /// <summary>
    /// Words run in any of the eight directions, including diagonals.
    /// </summary>
    Advanced
}
