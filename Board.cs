using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Strategies;

namespace Sudoku
{
    public partial class Board
    {
        public const int size = 9;
        public const int boxSize = 3;
        public Cell[] board;
        public List<Strategy> strategies;
        public const int DrawOffsetX = (Main.Width - boxSize * Cell.boxSize) / 2;
        public const int DrawOffsetY = 20;

        public Solution solution;
        private int solutionProgress;
        private Stack<HelpSnapshot> helpHistory = new Stack<HelpSnapshot>();
        private TextboxInfo previousTextbox = null;
        public TextboxInfo textbox = null;
        private float textboxTransition = 1f;
        public float transition = 1f;
        private long solveTime = 0;

        public Board()
        {
            this.board = new Cell[size * size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    this.board[x + size * y] = new Cell(this, x, y);
                }
            }
            InitializeButtons();
        }

        private void StartSolution()
        {
            foreach (Cell cell in board)
            {
                if (cell.info == null)
                {
                    cell.info = new Notes();
                }
            }
            strategies = new List<Strategy>();
            strategies.Add(new Elimination());
            strategies.Add(new RemainingSingle());
            strategies.Add(new IsolatedSingle());
            strategies.Add(new Trap());
            strategies.Add(new RemainingDouble());
            strategies.Add(new IsolatedDouble());
            strategies.Add(new TwoByTwo());
            strategies.Add(new ThreeByThree());
            strategies.Add(new SpikedTwoByTwo());
            strategies.Add(new SpikedThreeByThree());
            strategies.Add(new Ladder());
            strategies.Add(new RemainingTriple());
            strategies.Add(new IsolatedTriple());
            strategies.Add(new SuperDifferenceFinder());
        }

        public Solution Solve(Difficulty cap = Difficulty.All)
        {
            List<Snapshot> snapshots = new List<Snapshot>();
            snapshots.Add(new Snapshot(this));
            List<StrategyResult> steps = new List<StrategyResult>();
            bool finished;
            bool invalid;
            StrategyResult result = AttemptStrategy(out finished, out invalid, cap);
            while (result != null)
            {
                snapshots.Add(new Snapshot(this));
                steps.Add(result);
                result = AttemptStrategy(out finished, out invalid, cap);
            }
            Solution solution = new Solution(snapshots, steps, finished, invalid);
            snapshots[0].CopyTo(this);
            solutionProgress = 0;
            return solution;
        }

        private void DeletePuzzle()
        {
            foreach (Cell cell in board)
            {
                cell.info = null;
                cell.transitionColor = null;
                cell.overrideColor = null;
            }
            previousTextbox = null;
            textbox = null;
            solveTime = 0;
            strategies = null;
            solution = null;
            solutionProgress = 0;
            helpHistory.Clear();
        }

        public void SetupTransition()
        {
            foreach (Cell cell in board)
            {
                cell.previousInfo = cell.info;
                cell.info = cell.info.Clone();
                cell.transitionColor = cell.overrideColor;
                cell.overrideColor = null;
            }
            transition = 0f;
        }

        public void Reset()
        {
            SetupTransition();
            solutionProgress = 0;
            helpHistory.Clear();
            ChangeTextbox(null);
            solution.snapshots[solutionProgress].CopyTo(this);
        }

        public void Undo()
        {
            SetupTransition();
            solutionProgress--;
            if (solutionProgress < 0)
            {
                solutionProgress = 0;
            }
            helpHistory.Clear();
            ChangeTextbox(null);
            solution.snapshots[solutionProgress].CopyTo(this);
        }

        public void Next(bool transition = true)
        {
            if (transition)
            {
                SetupTransition();
            }
            solutionProgress++;
            TextboxInfo changeTextbox = null;
            if (transition && solutionProgress >= solution.snapshots.Count)
            {
                solutionProgress = solution.snapshots.Count - 1;
                if (solution.Completed)
                {
                    changeTextbox = new TextboxInfo(Main.textResource.completed);
                }
                else
                {
                    if (solution.Invalid)
                    {
                        changeTextbox = new TextboxInfo(Main.textResource.unsolvable);
                    }
                    else
                    {
                        changeTextbox = new TextboxInfo(Main.textResource.tooDifficult);
                    }
                }
            }
            helpHistory.Clear();
            if (transition)
            {
                ChangeTextbox(changeTextbox);
            }
            solution.snapshots[solutionProgress].CopyTo(this);
            if (transition)
            {
                foreach (Cell cell in board)
                {
                    if (cell.previousInfo is Notes)
                    {
                        Notes notes = (Notes)cell.previousInfo;
                        if (cell.info is Number)
                        {
                            cell.transitionColor = Color.Green;
                        }
                        else if (cell.info is Notes)
                        {
                            Notes newNotes = (Notes)cell.info;
                            for (int k = 0; k < CellInfo.numbers; k++)
                            {
                                if (notes.Values[k] && !newNotes.Values[k])
                                {
                                    notes.OverrideColors[k] = Color.Red;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Finish()
        {
            SetupTransition();
            solutionProgress = solution.snapshots.Count - 1;
            helpHistory.Clear();
            ChangeTextbox(null);
            solution.snapshots[solutionProgress].CopyTo(this);
        }

        public void UndoHelp()
        {
            if (helpHistory.Count > 0)
            {
                helpHistory.Pop().CopyTo(this);
            }
        }

        public void NextHelp()
        {
            if (solutionProgress == solution.snapshots.Count - 1)
            {
                Next();
                return;
            }
            helpHistory.Push(new HelpSnapshot(this));
            SetupTransition();
            ChangeTextbox(solution.steps[solutionProgress].Step(this, helpHistory.Count));
            if (textbox == null)
            {
                Next(false);
            }
        }

        public void ChangeTextbox(TextboxInfo next)
        {
            previousTextbox = textbox;
            textbox = next;
            textboxTransition = 0f;
        }

        public StrategyResult AttemptStrategy(out bool finished, out bool invalid, Difficulty cap = Difficulty.All)
        {
            invalid = false;
            if (PuzzleCompleted())
            {
                finished = true;
                return null;
            }
            finished = false;
            Snapshot snapshot = new Snapshot(board.Select(cell => cell.info.Clone()).ToArray());
            foreach (Strategy strategy in strategies)
            {
                if ((int)strategy.difficulty > (int)cap)
                {
                    break;
                }
                string outInfo = "";
                StrategyResult result = strategy.Apply(this, ref outInfo);
                if (result != null)
                {
                    if (!ValidatePuzzle())
                    {
                        snapshot.CopyTo(this);
                        invalid = true;
                        return null;
                    }
                    return result;
                }
            }
            return null;
        }

        public bool ValidatePuzzle()
        {
            for (int k = 0; k < CellInfo.numbers; k++)
            {
                if (!ValidateGroup(GetBox(k)))
                {
                    return false;
                }
                if (!ValidateGroup(GetRow(k)))
                {
                    return false;
                }
                if (!ValidateGroup(GetColumn(k)))
                {
                    return false;
                }
            }
            foreach (Cell cell in board)
            {
                if (cell.info != null && cell.info is Notes && !((Notes)cell.info).Validate())
                {
                    return false;
                }
            }
            return true;
        }

        private bool ValidateGroup(Cell[] cells)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                CellInfo info1 = cells[i].info;
                if (info1 != null && info1 is Number)
                {
                    for (int j = i + 1; j < cells.Length; j++)
                    {
                        CellInfo info2 = cells[j].info;
                        if (info2 != null && info2 is Number && info1.Equals(info2))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool PuzzleCompleted()
        {
            foreach (Cell cell in board)
            {
                if (!(cell.info is Number))
                {
                    return false;
                }
            }
            return true;
        }

        public Board Clone()
        {
            Board clone = new Board();
            for (int k = 0; k < this.board.Length; k++)
            {
                clone.board[k] = this.board[k].Clone(clone);
            }
            clone.strategies = new List<Strategy>(this.strategies);
            return clone;
        }

        public void Draw(Drawer drawer)
        {
            for (int x = 0; x < boxSize; x++)
            {
                for (int y = 0; y < boxSize; y++)
                {
                    int drawX = DrawOffsetX + Cell.boxSize * x;
                    int drawY = DrawOffsetY + Cell.boxSize * y;
                    drawer.Draw(drawer.box, drawX, drawY, Color.White);
                }
            }
            foreach (Cell cell in board)
            {
                cell.Draw(drawer);
            }
            DrawButtons(drawer);
            if (Paused && solveTime >= 5)
            {
                DrawSolvingBox(drawer);
            }
            DrawHelpBox(drawer);
        }

        private void DrawSolvingBox(Drawer drawer)
        {
            drawer.DrawTextBox(Main.Width / 2 - 100, Main.Height / 2 - 50, 200, 100);
            string text = Main.textResource.solving;
            float height = drawer.MeasureBigTextHeight(text);
            float[] widths = new float[text.Length];
            float totalWidth = 0f;
            float[] bounce = new float[text.Length];
            float bounceTime = solveTime % 120;
            float bounceDelay = 60f / (3f + text.Length);
            float bounceDuration = 4f * bounceDelay;
            for (int k = 0; k < text.Length; k++)
            {
                widths[k] = drawer.MeasureBigTextWidth(text[k].ToString());
                totalWidth += widths[k] + 1f;
                float letterBounce = bounceTime - 60f - k * bounceDelay;
                if (letterBounce >= 0 && letterBounce < bounceDuration)
                {
                    bounce[k] = 5f * (float)Math.Sin(letterBounce / bounceDuration * Math.PI);
                }
            }
            float x = Main.Width / 2 - totalWidth / 2f;
            float y = Main.Height / 2 - height / 2f;
            for (int k = 0; k < text.Length; k++)
            {
                drawer.DrawBigText((int)x, (int)(y - bounce[k]), text[k].ToString(), new Color(12, 12, 22));
                x += widths[k] + 1f;
            }
        }

        private void DrawHelpBox(Drawer drawer)
        {
            const float boxWidth = Main.Width - 160f;
            const float xOffset = Main.Width / 2 - boxWidth / 2f;
            const float yOffset = Main.Height - 140f;
            const float boxHeight = 120f;
            if (textbox != null)
            {
                if (textboxTransition < 1f)
                {
                    if (previousTextbox != null)
                    {
                        drawer.DrawTextBox(xOffset, yOffset, boxWidth, boxHeight, previousTextbox);
                        drawer.DrawTextBox(xOffset, yOffset, boxWidth, boxHeight, textbox, textboxTransition);
                    }
                    else
                    {
                        drawer.DrawTextBox(xOffset, yOffset + 140f * (1f - textboxTransition), boxWidth, boxHeight, textbox);
                    }
                }
                else
                {
                    drawer.DrawTextBox(xOffset, yOffset, boxWidth, boxHeight, textbox);
                }
            }
            else if (previousTextbox != null && textboxTransition < 1f)
            {
                drawer.DrawTextBox(xOffset, yOffset + 140f * textboxTransition, boxWidth, boxHeight, previousTextbox);
            }
        }

        public Cell GetCell(int x, int y)
        {
            return board[x + size * y];
        }

        public Cell[] GetBox(int box)
        {
            return GetBox(board, box);
        }

        public Cell[] GetRow(int row)
        {
            return GetRow(board, row);
        }
        
        public Cell[] GetColumn(int column)
        {
            return GetColumn(board, column);
        }

        public Cell[] GetBox(Cell[] cells, int box)
        {
            return cells.Where(cell => cell.Box == box).ToArray();
        }

        public Cell[] GetRow(Cell[] cells, int row)
        {
            return cells.Where(cell => cell.Row == row).ToArray();
        }

        public Cell[] GetColumn(Cell[] cells, int column)
        {
            return cells.Where(cell => cell.Column == column).ToArray();
        }
    }

    public class Cell
    {
        public const int size = 40;
        public const int boxSize = 3 * size + 2;

        private Board board;
        public CellInfo info;
        public CellInfo previousInfo;
        public Color? transitionColor;
        public Color? overrideColor;
        private int x;
        private int y;
        public int Box => (x / Board.boxSize) + Board.boxSize * (y / Board.boxSize);
        public int Row => y;
        public int Column => x;
        public int Index => x + Board.size * y;
        private int drawX;
        private int drawY;

        public Cell(Board board, int x, int y)
        {
            this.board = board;
            this.x = x;
            this.y = y;
            drawX = 1 + Board.DrawOffsetX + (x / 3) * boxSize + (x % 3) * size;
            drawY = 1 + Board.DrawOffsetY + (y / 3) * boxSize + (y % 3) * size;
        }

        public Cell Clone(Board newBoard)
        {
            Cell clone = new Cell(newBoard, this.x, this.y);
            clone.info = this.info.Clone();
            return clone;
        }

        public void Update()
        {
            Point mousePos = Main.input.MousePos;
            if (mousePos.X > drawX && mousePos.X < drawX + size && mousePos.Y > drawY && mousePos.Y < drawY + size)
            {
                board.hoverCell = this;
            }
        }

        public void Draw(Drawer drawer)
        {
            Color color = this == board.selectedCell ? Color.SelectedCell : Color.White;
            if (!board.settingUp && info is Number && ((Number)info).Starter)
            {
                color = Color.StartingCell;
            }
            if (board.hoverCell == this)
            {
                color *= 0.8f;
            }
            Color previousColor = transitionColor.HasValue ? transitionColor.Value : color;
            if (overrideColor.HasValue)
            {
                color = overrideColor.Value;
            }
            if (board.transition < 1f)
            {
                color = Color.Lerp(previousColor, color, board.transition);
            }
            drawer.Draw(drawer.cell, drawX, drawY, color);
            int buffer = (size - CellInfo.size) / 2;
            if (board.transition < 1f && previousInfo != null)
            {
                previousInfo.Draw(drawer, drawX + buffer, drawY + buffer, color);
            }
            if (info != null)
            {
                info.Draw(drawer, drawX + buffer, drawY + buffer, color, board.transition);
            }
        }
    }
}
