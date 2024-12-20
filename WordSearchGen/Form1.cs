using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WordSearchGen
{
    public partial class Form1 : Form
    {
        private WordSearchParams parameters;
        private int currentX;
        private int currentY;
        private readonly Dictionary<OrientationType.Orientation, int> orientations = new Dictionary<OrientationType.Orientation, int>();
        private readonly Dictionary<OrientationType.Orientation, Point> coordinates = Util.GetOrientationCoordinates();
        private bool squaresGenerated;

        private int MaxHeight { get; set; }

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            parameters = new WordSearchParams();
            currentX = currentY = parameters.Offset;
            SetupDictionary();
        }

        private void SetupDictionary()
        {
            for (var o = OrientationType.Orientation.D; o <= OrientationType.Orientation.UL; o++)
            {
                orientations.Add(o, 0);
            }
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            var boardDrawnOk = false;

            while (!boardDrawnOk)
            {
                boardDrawnOk = DrawTheBoard();
            }
        }

        private bool DrawTheBoard()
        {
            try
            {
                GenerateSquares();
                AllocateWords();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void GenerateSquares()
        {
            MaxHeight = 0;

            if (squaresGenerated)
            {
                ClearLabels();
            }
            else
            {
                var topEnd = parameters.TotalSquares;

                for (var l = 0; l < topEnd; l++)
                {
                    var drawnLabel = DrawLabel(l);
                    if (drawnLabel.Top > MaxHeight)
                    {
                        MaxHeight = drawnLabel.Top;
                    }
                }
                DrawWords();
                squaresGenerated = true;

            }
        }

        private void DrawWords()
        {
            int originalTop;
            var currentTop = originalTop = MaxHeight + parameters.SquareSize * 2;
            var currentLeft = parameters.Offset;
            var labelsAdded = 0;
            foreach (var w in parameters.GetWords())
            {
                var lab = new Label();
                lab.Top = currentTop;
                lab.Left = currentLeft;
                lab.Text = w.ToUpper();
                lab.Font = new Font(lab.Font, FontStyle.Bold);
                lab.ForeColor = Color.Blue;
                currentTop += parameters.SquareSize;
                Controls.Add(lab);
                labelsAdded++;

                if (labelsAdded % 5 == 0)
                {
                    currentLeft += 200;
                    currentTop = originalTop;
                }
            }
        }

        private void AllocateWords()
        {
            foreach (var w in parameters.GetWords())
            {
                var choices = new List<OrientationType.Orientation>();
                var foundAny = false;
                var index = 0;

                while (foundAny == false)
                {
                    var startPointFound = false;

                    while (startPointFound == false)
                    {
                        index = GetPossibleStartSquare();
                        if (!Util.SquareIsOccupied(GetCurrentBoard(), string.Empty, index))
                        {
                            startPointFound = true;
                        }
                    }

                    var options = PlaceFinder.GetOrientationOptions(coordinates, GetCurrentBoard(), w, parameters.GridSize, index);
                    if (options.Count == 0)
                        continue;
                    var candidates = GetLeastCommonOrientation();

                    choices = options.Intersect(candidates).ToList();

                    if (choices.Count == 0)
                    {
                        choices = new List<OrientationType.Orientation> { options[0] };
                    }
                    foundAny = choices.Any();
                }
                var choice = choices.FirstOrDefault();
                PlaceWord(choice, index, w);
            }
        }

        private void PlaceWord(OrientationType.Orientation orientation, int index, string word)
        {
            var lettersLeft = word.Length;
            var currentLetter = 0;

            while (lettersLeft > 0)
            {
                var theLetter = word.Substring(currentLetter, 1);
                var lab = GetLabelByIndex(index);

                if (lab.Text != string.Empty)
                {
                    throw new ArgumentException("Square was not empty!");
                }

                lab.Text = theLetter.ToUpper();
                var x = Util.GetOrientationCoordinates()[orientation].X;
                var y = Util.GetOrientationCoordinates()[orientation].Y;

                index += x + y * parameters.GridSize;
                lettersLeft--;
                currentLetter++;
                Application.DoEvents();
            }
            orientations[orientation] += 1;
        }

        private List<OrientationType.Orientation> GetLeastCommonOrientation()
        {
            var min = orientations.Min(x => x.Value);
            var seldomUsed = orientations.Where(x => x.Value == min).Select(y => y.Key);

            return seldomUsed.ToList();
        }

        private Label DrawLabel(int index)
        {
            var label = GetRawLabel(index);
            label.Height = label.Width = parameters.SquareSize;
            label.Left = currentX;
            label.Top = currentY;
            Controls.Add(label);

            var addAmount = parameters.SquareSize - parameters.Padding;

            if ((index + parameters.Padding) % parameters.GridSize == 0)
            {
                currentY += addAmount;
                currentX = parameters.Offset;
            }
            else
            {
                currentX += addAmount;
            }

            return label;
        }

        private Label GetRawLabel(int tag)
        {
            var retVal = new Label();
            retVal.Font = new Font(retVal.Font, FontStyle.Bold);
            retVal.TextAlign = ContentAlignment.MiddleCenter;
            retVal.BorderStyle = BorderStyle.FixedSingle;
            retVal.ForeColor = Color.Blue;
            retVal.Tag = tag + parameters.Padding;
            return retVal;

        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private int GetPossibleStartSquare()
        {
            var currentLetter = "#";
            var square = 0;

            while (currentLetter != string.Empty)
            {
                var candidate = Util.GetNextStep();
                square = candidate % parameters.TotalSquares;
                var label = GetLabelByNumber(square + 1);
                currentLetter = label.Text;
            }

            return square + 1;
        }

        private void ClearLabels()
        {
            foreach (var l in Controls)
            {
                if (l is Label)
                {
                    var currentLab = l as Label;

                    if (currentLab.Tag != null)
                    {
                        currentLab.Text = string.Empty;
                    }
                }
            }
        }

        private Label GetLabelByIndex(int index)
        {
            foreach (var l in Controls)
            {
                if (l is Label currentLab)
                {
                    if (Convert.ToInt32(currentLab.Tag) == index)
                    {
                        return currentLab;
                    }
                }
            }
            return null;
        }

        private List<string> GetCurrentBoard()
        {
            var retVal = new List<string>();

            foreach (var l in Controls)
            {
                if (l is Label currentLab)
                {
                    retVal.Add(currentLab.Text);
                }
            }

            return retVal;
        }

        private Label GetLabelByNumber(int index)
        {
            foreach (var l in Controls)
            {
                if (l is Label currentLab)
                {
                    if (Convert.ToInt32(currentLab.Tag) == index)
                    {
                        return currentLab;
                    }
                }
            }

            return null;
        }

        private void FillGrid_Click(object sender, EventArgs e)
        {
            foreach (var l in Controls)
            {
                if (l is Label currentLab)
                {
                    if (currentLab.Tag != null && currentLab.Text == string.Empty)
                    {
                        currentLab.Text = Util.GetRandomLetter();
                    }
                }
            }
        }
    }
}
