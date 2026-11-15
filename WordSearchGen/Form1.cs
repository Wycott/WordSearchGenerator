using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WordSearchGen.Core;

namespace WordSearchGen
{
    public partial class Form1 : Form
    {
        private readonly WordSearchParams parameters = new WordSearchParams();
        private readonly IReadOnlyList<string> words = WordLists.Current;
        private readonly Random random = Random.Shared;
        private readonly Font boldFont;
        private readonly Label[,] squares;
        private readonly WordSearchGenerator generator;
        private WordSearch puzzle;

        public Form1()
        {
            InitializeComponent();

            boldFont = new Font(Font, FontStyle.Bold);
            squares = new Label[parameters.GridSize, parameters.GridSize];
            generator = new WordSearchGenerator(parameters.GridSize, random);

            DrawSquares();
            DrawWords();
            FillGrid.Enabled = false;
        }

        private void DrawSquares()
        {
            var step = parameters.SquareSize - parameters.CellOverlap;

            for (var row = 0; row < parameters.GridSize; row++)
            {
                for (var column = 0; column < parameters.GridSize; column++)
                {
                    var label = new Label
                    {
                        Font = boldFont,
                        TextAlign = ContentAlignment.MiddleCenter,
                        BorderStyle = BorderStyle.FixedSingle,
                        ForeColor = Color.Blue,
                        Width = parameters.SquareSize,
                        Height = parameters.SquareSize,
                        Left = parameters.Offset + column * step,
                        Top = parameters.Offset + row * step
                    };

                    squares[row, column] = label;
                    Controls.Add(label);
                }
            }
        }

        private void DrawWords()
        {
            var step = parameters.SquareSize - parameters.CellOverlap;
            var gridBottom = parameters.Offset + (parameters.GridSize - 1) * step;
            var firstTop = gridBottom + parameters.SquareSize * 2;

            for (var i = 0; i < words.Count; i++)
            {
                var column = i / parameters.WordsPerColumn;
                var rowInColumn = i % parameters.WordsPerColumn;

                Controls.Add(new Label
                {
                    Font = boldFont,
                    ForeColor = Color.Blue,
                    Text = words[i].ToUpperInvariant(),
                    Left = parameters.Offset + column * parameters.WordColumnWidth,
                    Top = firstTop + rowInColumn * parameters.SquareSize
                });
            }
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            try
            {
                puzzle = generator.Generate(words);
            }
            catch (Exception ex) when (ex is WordSearchGenerationException || ex is ArgumentException)
            {
                MessageBox.Show(this, ex.Message, "Could not generate puzzle", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ShowGrid();
            FillGrid.Enabled = true;
        }

        private void FillGrid_Click(object sender, EventArgs e)
        {
            if (puzzle == null)
            {
                return;
            }

            puzzle.Grid.FillEmpty(random, LetterDistribution.English);
            ShowGrid();
        }

        private void ShowGrid()
        {
            var grid = puzzle.Grid;

            for (var row = 0; row < grid.Size; row++)
            {
                for (var column = 0; column < grid.Size; column++)
                {
                    squares[row, column].Text = grid.IsEmpty(row, column)
                        ? string.Empty
                        : grid[row, column].ToString();
                }
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            boldFont.Dispose();
            base.OnFormClosed(e);
        }
    }
}
