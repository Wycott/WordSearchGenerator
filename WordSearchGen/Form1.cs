using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WordSearchGen.Core;

namespace WordSearchGen
{
    public sealed partial class Form1 : Form
    {
        private readonly WordSearchParams parameters = new();
        private readonly IReadOnlyList<string> words = WordLists.Current;
        private readonly Random random = Random.Shared;
        private readonly Font boldFont;
        private readonly Label[,] squares;
        private readonly List<Label> wordLabels = new();
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
            CentreLayout();
            FillGrid.Enabled = false;
        }

        /// <summary>
        /// Stack the skill level, buttons, grid and word list, centre each one horizontally
        /// and centre the whole stack vertically on the form.
        /// </summary>
        private void CentreLayout()
        {
            var step = parameters.SquareSize - parameters.CellOverlap;
            var gridExtent = (parameters.GridSize - 1) * step + parameters.SquareSize;

            Button[] buttons = [Generate, FillGrid, Exit];
            var buttonsWidth = (buttons.Length - 1) * parameters.ButtonGap;
            var buttonsHeight = 0;

            foreach (var button in buttons)
            {
                buttonsWidth += button.Width;
                buttonsHeight = Math.Max(buttonsHeight, button.Height);
            }

            var wordsWidth = 0;
            var wordsHeight = 0;

            foreach (var label in wordLabels)
            {
                wordsWidth = Math.Max(wordsWidth, label.Left + label.Width);
                wordsHeight = Math.Max(wordsHeight, label.Top + label.Height);
            }

            var totalHeight = SkillLevelGroup.Height
                + parameters.ButtonGap + buttonsHeight
                + parameters.SectionGap + gridExtent
                + (wordLabels.Count > 0 ? parameters.SectionGap + wordsHeight : 0);

            var top = Math.Max(0, (ClientSize.Height - totalHeight) / 2);

            SkillLevelGroup.Location = new Point(CentredLeft(SkillLevelGroup.Width), top);
            top += SkillLevelGroup.Height + parameters.ButtonGap;

            var buttonLeft = CentredLeft(buttonsWidth);

            foreach (var button in buttons)
            {
                button.Location = new Point(buttonLeft, top);
                buttonLeft += button.Width + parameters.ButtonGap;
            }

            top += buttonsHeight + parameters.SectionGap;

            var gridLeft = CentredLeft(gridExtent);

            for (var row = 0; row < parameters.GridSize; row++)
            {
                for (var column = 0; column < parameters.GridSize; column++)
                {
                    squares[row, column].Location = new Point(gridLeft + column * step, top + row * step);
                }
            }

            top += gridExtent + parameters.SectionGap;

            // Word labels were created relative to (0, 0); shift the block as a whole.
            var wordsLeft = CentredLeft(wordsWidth);

            foreach (var label in wordLabels)
            {
                label.Location = new Point(wordsLeft + label.Left, top + label.Top);
            }
        }

        private int CentredLeft(int width) => Math.Max(0, (ClientSize.Width - width) / 2);

        private void DrawSquares()
        {
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
                        Height = parameters.SquareSize
                    };

                    squares[row, column] = label;
                    Controls.Add(label);
                }
            }
        }

        /// <summary>
        /// Add the word list labels, positioned relative to the top left of the list.
        /// <see cref="CentreLayout"/> moves them into place.
        /// </summary>
        private void DrawWords()
        {
            for (var i = 0; i < words.Count; i++)
            {
                var column = i / parameters.WordsPerColumn;
                var rowInColumn = i % parameters.WordsPerColumn;

                var label = new Label
                {
                    AutoSize = true,
                    Font = boldFont,
                    ForeColor = Color.Blue,
                    Text = words[i].ToUpperInvariant(),
                    Left = column * parameters.WordColumnWidth,
                    Top = rowInColumn * parameters.SquareSize
                };

                wordLabels.Add(label);
                Controls.Add(label);
            }
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            try
            {
                puzzle = generator.Generate(words, SelectedSkillLevel());
            }
            catch (Exception ex) when (ex is WordSearchGenerationException || ex is ArgumentException)
            {
                MessageBox.Show(this, ex.Message, @"Could not generate puzzle", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ShowGrid();
            FillGrid.Enabled = true;
        }

        private SkillLevel SelectedSkillLevel()
        {
            if (Beginner.Checked)
            {
                return SkillLevel.Beginner;
            }

            return Intermediate.Checked ? SkillLevel.Intermediate : SkillLevel.Advanced;
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
