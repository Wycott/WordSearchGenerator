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
        /// Offset in pixels (both x and y) from the top left of the form to the grid
        /// </summary>
        internal int Offset { get; } = 100;

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
