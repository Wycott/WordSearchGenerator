using System.Collections.Generic;
using System.Drawing;

namespace WordSearchGen
{
    internal static class PlaceFinder
    {
        /// <summary>
        /// See if there is room for a word in each direction, and if so, add it to the list
        /// </summary>
        /// <param name="coordinates"></param>
        /// <param name="currentBoard"></param>
        /// <param name="word"></param>
        /// <param name="side"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        internal static List<OrientationType.Orientation> GetOrientationOptions(Dictionary<OrientationType.Orientation, Point> coordinates, List<string> currentBoard, string word, int side, int startPos)
        {
            var retVal = new List<OrientationType.Orientation>();
            foreach (var o in coordinates.Keys)
            {
                if (IsRoomForWord(currentBoard, word, side, startPos, coordinates[o].X, coordinates[o].Y))
                {
                    retVal.Add(o);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Check if there is room for a word in the supplied orientation (in co-ordinate form)
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="word"></param>
        /// <param name="side"></param>
        /// <param name="startPos"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool IsRoomForWord(List<string> currentBoard, string word, int side, int startPos, int x, int y)
        {
            var lettersRemaining = word.Length - 1;
            var currentPos = startPos;
            var currentLetter = 0;
            while (lettersRemaining > 0)
            {
                var wordLetter = word.Substring(currentLetter, 1);

                if (Util.SquareIsOccupied(currentBoard, wordLetter, currentPos))
                {
                    return false;
                }

                if (x > 0)
                {
                    if (AtRightEdge(side, currentPos))
                    {
                        return false;
                    }
                }

                if (x < 0)
                {
                    if (AtLeftEdge(side, currentPos))
                    {
                        return false;
                    }
                }

                if (y > 0)
                {
                    if (AtBottomEdge(side, currentPos))
                    {
                        return false;
                    }
                }

                if (y < 0)
                {
                    if (AtTopEdge(side, currentPos))
                    {
                        return false;
                    }
                }

                lettersRemaining--;
                currentLetter++;
                currentPos += x + side * y;
            }
            return true;
        }

        /// <summary>
        /// Check to see if we're at the left edge of the grid
        /// </summary>
        /// <param name="side"></param>
        /// <param name="currentPos"></param>
        /// <returns></returns>
        private static bool AtLeftEdge(int side, int currentPos)
        {
            var candidates = new List<int>();
            for (var s = 1; s <= side * side; s++)
            {
                if (s % side == 0)
                {
                    candidates.Add(s - side + 1);
                }
            }
            return candidates.Contains(currentPos);
        }

        /// <summary>
        /// Check to see if we're at the right edge of the grid
        /// </summary>
        /// <param name="side"></param>
        /// <param name="currentPos"></param>
        /// <returns></returns>
        private static bool AtRightEdge(int side, int currentPos)
        {
            var candidates = new List<int>();
            for (var s = 1; s <= side * side; s++)
            {
                if (s % side == 0)
                {
                    candidates.Add(s);
                }
            }
            return candidates.Contains(currentPos);
        }

        /// <summary>
        /// Check to see if we're at the top edge of the grid
        /// </summary>
        /// <param name="side"></param>
        /// <param name="currentPos"></param>
        /// <returns></returns>
        private static bool AtTopEdge(int side, int currentPos)
        {
            return currentPos > 0 && currentPos <= side;
        }

        /// <summary>
        /// Check to see if we're at the bottom edge of the grid
        /// </summary>
        /// <param name="side"></param>
        /// <param name="currentPos"></param>
        /// <returns></returns>
        private static bool AtBottomEdge(int side, int currentPos)
        {
            var topEnd = side * side;

            return currentPos > topEnd - (side + 1) && currentPos <= topEnd;
        }
    }
}
