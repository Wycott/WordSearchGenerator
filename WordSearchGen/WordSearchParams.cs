using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordSearchGen
{
    internal class WordSearchParams
    {
        internal WordSearchParams()
        {
            GridSize = 12;
            Offset = 100;
            SquareSize = 25;
            Padding = 1;                                               
        }

        /// <summary>
        /// Length (in letters) of the side of the grid
        /// </summary>
        internal int GridSize { get; set; }

        /// <summary>
        /// Offset (both x & y) from the top right of the form where the grid is formed
        /// </summary>
        internal int Offset { get; set; }

        /// <summary>
        /// Square size (letter container)
        /// </summary>
        internal int SquareSize { get; set; }

        /// <summary>
        /// Amount of padding between each label forming the cell
        /// </summary>
        internal int Padding { get; set; }

        /// <summary>
        /// Number of squares on the board
        /// </summary>
        internal int TotalSquares
        {
            get
            {
                return GridSize * GridSize;
            }
        }

        /// <summary>
        /// Convention here is to keep old ones for
        ///     a) Posterity
        ///     b) Testing
        ///     c) Recreating old puzzles
        /// </summary>
        /// <returns></returns>
        internal List<String> GetWords()
        {
            // TODO: Add more word lists here
            List<string> retVal = new List<string>();

            //retVal.Add("ant");
            //retVal.Add("ladybird");
            //retVal.Add("butterfly");
            //retVal.Add("spider");
            //retVal.Add("centipede");
            //retVal.Add("caterpillar");
            //retVal.Add("beetle");
            //retVal.Add("cricket");
            //retVal.Add("woodlouse");
            //retVal.Add("wasp");

            //retVal.Add("pizza");
            //retVal.Add("chips");
            //retVal.Add("sausage");
            //retVal.Add("beans");
            //retVal.Add("chocolate");
            //retVal.Add("crisps");
            //retVal.Add("doughnut");
            //retVal.Add("cake");
            //retVal.Add("milkshake");
            //retVal.Add("haribos");

            retVal.Add("superman");
            retVal.Add("starfire");
            retVal.Add("raven");
            retVal.Add("penguin");
            retVal.Add("nightwing");
            retVal.Add("krypto");
            retVal.Add("joker");
            retVal.Add("cyborg");
            retVal.Add("batman");
            retVal.Add("beastboy");

            return retVal;

        }        
    }
}
