namespace WordSearchGen
{
    internal class OrientationType
    {
        /// <summary>
        /// List of directions that a word can take on the grid
        /// </summary>
        internal enum Orientation
        {
            D = 0,
            R,
            U,
            L,
            UR,
            DR,
            DL,
            UL
        }
    }
}
