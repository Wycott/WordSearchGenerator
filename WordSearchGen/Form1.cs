using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WordSearchGen.Core;

namespace WordSearchGen
{
    public sealed partial class Form1 : Form
    {
        private const double DefaultHighlightSeconds = 3;
        private const double DefaultFlashMilliseconds = 250;
        private const float HighlightFontScale = 1.4f;
        private static readonly Color LetterColour = Color.Blue;
        private static readonly Color HighlightColour = Color.Red;

        private readonly WordSearchParams parameters = new();
        private readonly IReadOnlyList<string> words = WordLists.Current;
        private readonly Random random = Random.Shared;
        private readonly Font boldFont;
        private readonly Font highlightFont;
        private readonly Label[,] squares;
        private readonly List<Label> wordLabels = new();
        private readonly WordSearchGenerator generator;
        private readonly Timer flashTimer;
        private readonly Stopwatch flashClock = new();
        private readonly int highlightMilliseconds;
        private readonly HashSet<Label> highlightedLabels = new();
        private readonly Dictionary<Label, int> flashingClueTops = new();
        private bool flashOn;
        private WordSearch puzzle;

        public Form1()
        {
            InitializeComponent();

            boldFont = new Font(Font, FontStyle.Bold);
            highlightFont = new Font(Font.FontFamily, Font.Size * HighlightFontScale, FontStyle.Bold, Font.Unit);
            squares = new Label[parameters.GridSize, parameters.GridSize];
            generator = new WordSearchGenerator(parameters.GridSize, random);

            highlightMilliseconds = MillisecondsSetting(
                () => Properties.Settings.Default.HighlightSeconds * 1000, DefaultHighlightSeconds * 1000);
            flashTimer = new Timer
            {
                Interval = MillisecondsSetting(() => Properties.Settings.Default.FlashMilliseconds, DefaultFlashMilliseconds)
            };
            flashTimer.Tick += FlashTimer_Tick;

            DrawSquares();
            DrawWords();
            CentreLayout();
            RadioFor(SavedSkillLevel()).Checked = true;
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
                        ForeColor = LetterColour,
                        Width = parameters.SquareSize,
                        Height = parameters.SquareSize
                    };

                    label.Click += Square_Click;
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
                    ForeColor = LetterColour,
                    Text = words[i].ToUpperInvariant(),
                    Tag = words[i],
                    Cursor = Cursors.Hand,
                    Left = column * parameters.WordColumnWidth,
                    Top = rowInColumn * parameters.SquareSize
                };

                label.Click += Word_Click;

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

            ClearHighlight();
            ShowGrid();
            FillGrid.Enabled = true;
        }

        /// <summary>
        /// Show where the clicked clue is hidden. Clicking another clue moves the highlight to it.
        /// </summary>
        private void Word_Click(object sender, EventArgs e)
        {
            Highlight([(Label)sender]);
        }

        /// <summary>
        /// Easter egg: clicking anywhere on the grid shows where every word is hidden.
        /// </summary>
        private void Square_Click(object sender, EventArgs e)
        {
            Highlight(wordLabels);
        }

        /// <summary>
        /// Flash the given clues, and their letters in the grid, in a larger bold font until
        /// the highlight time runs out. Does nothing until a puzzle has been generated.
        /// </summary>
        private void Highlight(IEnumerable<Label> clues)
        {
            if (puzzle == null)
            {
                return;
            }

            ClearHighlight();

            foreach (var clue in clues)
            {
                var placement = puzzle.Find((string)clue.Tag);

                if (placement == null)
                {
                    continue;
                }

                // The clue sizes itself to its text and grows downwards, so move it up by half
                // the extra height to keep it centred between its neighbours.
                var normalHeight = clue.Height;
                flashingClueTops[clue] = clue.Top;
                clue.Font = highlightFont;
                clue.Top -= (clue.Height - normalHeight) / 2;
                highlightedLabels.Add(clue);

                foreach (var (row, column) in placement.Cells())
                {
                    var square = squares[row, column];
                    square.Font = highlightFont;
                    highlightedLabels.Add(square);
                }
            }

            if (highlightedLabels.Count == 0)
            {
                return;
            }

            flashOn = true;
            ShowFlash();
            flashClock.Restart();
            flashTimer.Start();
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            if (flashClock.ElapsedMilliseconds >= highlightMilliseconds)
            {
                ClearHighlight();
                return;
            }

            flashOn = !flashOn;
            ShowFlash();
        }

        /// <summary>
        /// Draw the highlighted letters in the highlight colour, or in their background
        /// colour (so they briefly vanish) on the "off" half of each flash.
        /// </summary>
        private void ShowFlash()
        {
            foreach (var label in highlightedLabels)
            {
                label.ForeColor = flashOn ? HighlightColour : label.BackColor;
            }
        }

        private void ClearHighlight()
        {
            flashTimer.Stop();
            flashClock.Reset();

            foreach (var label in highlightedLabels)
            {
                label.ForeColor = LetterColour;
                label.Font = boldFont;
            }

            highlightedLabels.Clear();

            foreach (var (clue, top) in flashingClueTops)
            {
                clue.Top = top;
            }

            flashingClueTops.Clear();
        }

        /// <summary>
        /// Read a duration setting from the app config, falling back to
        /// <paramref name="fallback"/> if it is missing, unreadable or not positive.
        /// </summary>
        private static int MillisecondsSetting(Func<double> read, double fallback)
        {
            double milliseconds;

            try
            {
                milliseconds = read();
            }
            catch (Exception ex) when (ex is ConfigurationException || ex is SettingsPropertyWrongTypeException)
            {
                milliseconds = fallback;
            }

            if (!double.IsFinite(milliseconds) || milliseconds <= 0)
            {
                milliseconds = fallback;
            }

            return (int)Math.Clamp(milliseconds, 1, int.MaxValue);
        }

        private SkillLevel SelectedSkillLevel()
        {
            if (Beginner.Checked)
            {
                return SkillLevel.Beginner;
            }

            return Intermediate.Checked ? SkillLevel.Intermediate : SkillLevel.Advanced;
        }

        private RadioButton RadioFor(SkillLevel level) => level switch
        {
            SkillLevel.Beginner => Beginner,
            SkillLevel.Intermediate => Intermediate,
            _ => Advanced
        };

        /// <summary>
        /// The skill level saved when the app was last closed, or Advanced if there is none
        /// or it cannot be read.
        /// </summary>
        private static SkillLevel SavedSkillLevel()
        {
            try
            {
                if (Enum.TryParse<SkillLevel>(Properties.Settings.Default.SkillLevel, true, out var level)
                    && Enum.IsDefined(level))
                {
                    return level;
                }
            }
            catch (ConfigurationException)
            {
                // Fall through to the default.
            }

            return SkillLevel.Advanced;
        }

        /// <summary>
        /// Remember the chosen skill level for next time, in the user's user.config.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.Cancel)
            {
                return;
            }

            try
            {
                Properties.Settings.Default.SkillLevel = SelectedSkillLevel().ToString();
                Properties.Settings.Default.Save();
            }
            catch (ConfigurationException)
            {
                // Not being able to save the preference is no reason to stop the app closing.
            }
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
            flashTimer.Dispose();
            highlightFont.Dispose();
            boldFont.Dispose();
            base.OnFormClosed(e);
        }
    }
}
