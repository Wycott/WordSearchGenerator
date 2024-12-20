using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
namespace WordSearchGen
{
    internal class Util
    {
        /// <summary>
        /// Get a random(ish) number between 0 & 999
        /// </summary>
        /// <returns></returns>
        internal static int GetNextStep()
        {
            var rawData = Guid.NewGuid().ToString();

            return Convert.ToInt32(new string(rawData.Where(char.IsDigit).ToArray()).Substring(0, 3));
        }

        /// <summary>
        /// Check if the square is occupied (or invalid)
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="wordLetter"></param>
        /// <param name="currentPos"></param>
        /// <returns></returns>
        internal static bool SquareIsOccupied(List<string> currentBoard, string wordLetter, int currentPos)
        {
            if (currentPos < 1 || currentPos > currentBoard.Count)
            {
                return true;
            }

            return currentBoard[currentPos - 1] != string.Empty && currentBoard[currentPos - 1] != wordLetter;
        }

        /// <summary>
        /// Get a random letter (for filling up the grid once the words have been laid down)
        /// </summary>
        /// <returns></returns>
        internal static string GetRandomLetter()
        {
            var seed = GetNextStep();
            var letterIndex = seed % 26;

            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(letterIndex, 1);
        }

        /// <summary>
        /// Get a dictionary of directions in co-ordinate terms
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<OrientationType.Orientation, Point> GetOrientationCoordinates()
        {
            var retVal = new Dictionary<OrientationType.Orientation, Point>();
            for (var o = OrientationType.Orientation.D; o <= OrientationType.Orientation.UL; o++)
            {
                int x;
                int y;
                switch (o)
                {
                    case OrientationType.Orientation.D:
                        x = 0;
                        y = 1;
                        retVal.Add(o, new Point(x, y));
                        break;
                    case OrientationType.Orientation.R:
                        x = 1;
                        y = 0;
                        retVal.Add(o, new Point(x, y));
                        break;
                    case OrientationType.Orientation.U:
                        x = 0;
                        y = -1;
                        retVal.Add(o, new Point(x, y));
                        break;
                    case OrientationType.Orientation.L:
                        x = -1;
                        y = 0;
                        retVal.Add(o, new Point(x, y));
                        break;
                    case OrientationType.Orientation.UR:
                        x = 1;
                        y = -1;
                        retVal.Add(o, new Point(x, y));
                        break;
                    case OrientationType.Orientation.DR:
                        x = 1;
                        y = 1;
                        retVal.Add(o, new Point(x, y));
                        break;
                    case OrientationType.Orientation.DL:
                        x = -1;
                        y = 1;
                        retVal.Add(o, new Point(x, y));
                        break;
                    case OrientationType.Orientation.UL:
                        x = -1;
                        y = -1;
                        retVal.Add(o, new Point(x, y));
                        break;
                }
            }

            return retVal;
        }
    }
}
