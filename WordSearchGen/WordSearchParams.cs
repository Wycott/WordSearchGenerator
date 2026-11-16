namespace WordSearchGen
{
    /// <summary>
    /// Size and on-screen layout of the puzzle.
    /// </summary>
    internal sealed class WordSearchParams
    {
        /// <summary>
        /// Length (in letters) of each side of the grid
        /// </summary>
        internal int GridSize { get; } = 12;

        /// <summary>
        /// Vertical gap in pixels between the controls, the grid and the word list
        /// </summary>
        internal int SectionGap { get; } = 20;

        /// <summary>
        /// Gap in pixels between neighbouring buttons, and between the skill level box and the buttons
        /// </summary>
        internal int ButtonGap { get; } = 6;

        /// <summary>
        /// Width and height in pixels of each square
        /// </summary>
        internal int SquareSize { get; } = 25;

        /// <summary>
        /// Pixels by which neighbouring squares overlap, so their borders form a single line
        /// </summary>
        internal int CellOverlap { get; } = 1;

        /// <summary>
        /// Number of words shown in each column of the word list
        /// </summary>
        internal int WordsPerColumn { get; } = 5;

        /// <summary>
        /// Horizontal distance in pixels between word list columns
        /// </summary>
        internal int WordColumnWidth { get; } = 200;
    }
}
