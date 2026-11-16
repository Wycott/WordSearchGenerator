namespace WordSearchGen.Core;

/// <summary>
/// Maps each <see cref="Orientation"/> to its row/column step.
/// </summary>
public static class Directions
{
    /// <summary>
    /// Every orientation, in declaration order.
    /// </summary>
    public static IReadOnlyList<Orientation> All { get; } = Enum.GetValues<Orientation>();

    private static readonly IReadOnlyList<Orientation> BeginnerOrientations =
        [Orientation.Down, Orientation.Right];

    private static readonly IReadOnlyList<Orientation> IntermediateOrientations =
        [Orientation.Down, Orientation.Right, Orientation.Up, Orientation.Left];

    /// <summary>
    /// The orientations words may use at the given skill level, in declaration order.
    /// </summary>
    public static IReadOnlyList<Orientation> For(SkillLevel level) => level switch
    {
        SkillLevel.Beginner => BeginnerOrientations,
        SkillLevel.Intermediate => IntermediateOrientations,
        SkillLevel.Advanced => All,
        _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
    };

    /// <summary>
    /// The change in row and column for one step in the given orientation.
    /// Rows increase downwards and columns increase to the right.
    /// </summary>
    public static (int RowStep, int ColumnStep) StepOf(Orientation orientation) => orientation switch
    {
        Orientation.Down => (1, 0),
        Orientation.Right => (0, 1),
        Orientation.Up => (-1, 0),
        Orientation.Left => (0, -1),
        Orientation.UpRight => (-1, 1),
        Orientation.DownRight => (1, 1),
        Orientation.DownLeft => (1, -1),
        Orientation.UpLeft => (-1, -1),
        _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
    };
}
