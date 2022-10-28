using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WordSearchGen
{
    public partial class Form1 : Form
    {
        private WordSearchParams parameters;
        private int CurrentX;
        private int CurrentY;
        private Dictionary<OrientationType.Orientation, int> orientations = new Dictionary<OrientationType.Orientation, int>();
        private Dictionary<OrientationType.Orientation, Point> coordinates = Util.GetOrientationCoordinates();
        private Boolean squaresGenerated = false;

        private int MaxHeight { get; set; }

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            parameters = new WordSearchParams();
            CurrentX = CurrentY = parameters.Offset;
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
            Boolean boardDrawnOK = false;

            while (!boardDrawnOK)
            {
                boardDrawnOK = DrawTheBoard();
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

                for (int l = 0; l < topEnd; l++)
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
            int currentTop = originalTop = MaxHeight + (parameters.SquareSize * 2);
            int currentLeft = parameters.Offset;
            int labelsAdded = 0;
            foreach (var w in parameters.GetWords())
            {
                var lab = new Label();
                lab.Top = currentTop;
                lab.Left = currentLeft;
                lab.Text = w.ToUpper();
                lab.Font = new Font(lab.Font, FontStyle.Bold);
                lab.ForeColor = Color.Blue;
                currentTop += parameters.SquareSize;
                this.Controls.Add(lab);
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
                List<OrientationType.Orientation> choices = new List<OrientationType.Orientation>();
                var wordSize = w.Length;
                bool foundAny = false;
                int index = 0;
                OrientationType.Orientation choice;

                while (foundAny == false)
                {
                    bool startPointFound = false;
                    
                    while (startPointFound == false)
                    {         
                        index = GetPossibleStartSquare();
                        if (!Util.SquareIsOccupied(GetCurrentBoard(), String.Empty, index))
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
                choice = choices.FirstOrDefault();
                PlaceWord(choice, index, w);
            }
        }

        private void PlaceWord(OrientationType.Orientation orientation, int index, string word)
        {
            int lettersLeft = word.Length;
            int currentLetter = 0;
            while(lettersLeft > 0)
            {
                string theLetter = word.Substring(currentLetter, 1);
                var lab = GetLabelByIndex(index);
                if (lab.Text != String.Empty)
                {
                    throw new System.ArgumentException("Square was not empty!");
                }
                lab.Text = theLetter.ToUpper();
                int x = Util.GetOrientationCoordinates()[orientation].X;
                int y = Util.GetOrientationCoordinates()[orientation].Y;

                index += x + (y * parameters.GridSize);
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
            label.Left = CurrentX;
            label.Top = CurrentY;
            this.Controls.Add(label);            

            int addAmount = parameters.SquareSize - parameters.Padding;

            if (((index + parameters.Padding) % parameters.GridSize) == 0)
            {
                CurrentY += addAmount;
                CurrentX = parameters.Offset;
            }
            else
            {
                CurrentX += addAmount;
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
            this.Close();
        }

        private int GetPossibleStartSquare()
        {
            var currentLetter = "#";
            int square = 0;
            while (currentLetter != String.Empty)
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
            foreach (var l in this.Controls)
            {
                if (l is Label)
                {
                    Label currentLab = l as Label;
                    if (currentLab.Tag != null)
                    {
                        currentLab.Text = String.Empty;
                    }
                }
            }
        }

        private Label GetLabelByIndex(int index)
        {
            foreach (var l in this.Controls)
            {
                if (l is Label)
                {
                    Label currentLab = l as Label;
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
            List<string> retVal = new List<string>();
            foreach (var l in this.Controls)
            {
                if (l is Label)
                {
                    Label currentLab = l as Label;
                    retVal.Add(currentLab.Text);
                }
            }
            return retVal;
        }

        private Label GetLabelByNumber(int index)
        {
            foreach( var l in this.Controls)
            {
                if (l is Label)
                {
                    Label currentLab = l as Label;
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
            foreach (var l in this.Controls)
            {
                if (l is Label)
                {
                    Label currentLab = l as Label;
                    if (currentLab.Tag != null && currentLab.Text == string.Empty)
                    {
                        currentLab.Text = Util.GetRandomLetter();
                    }
                }
            }
        }
    }
}
