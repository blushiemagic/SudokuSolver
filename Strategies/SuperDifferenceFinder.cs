using System;
using System.Collections.Generic;

namespace Sudoku.Strategies
{
    public class SuperDifferenceFinder : Strategy
    {
        public override Difficulty difficulty => Difficulty.Computer;

        public override StrategyResult Apply(Board board, ref string outInfo)
        {
            BinaryDecision[] decisions = GetDecisions(board).ToArray();
            foreach (Difficulty difficulty in Enum.GetValues(typeof(Difficulty)))
            {
                foreach (BinaryDecision decision in decisions)
                {
                    if (TryDecision(board, decision, difficulty))
                    {
                        outInfo = " (" + difficulty.ToString() + ")";
                        return new SuperDifferenceFinderResult();
                    }
                }
            }
            return null;
        }

        private List<BinaryDecision> GetDecisions(Board board)
        {
            List<BinaryDecision> decisions = new List<BinaryDecision>();
            for (int k = 0; k < Board.size; k++)
            {
                decisions.AddRange(GetDecisions(board.GetBox(k)));
                decisions.AddRange(GetDecisions(board.GetRow(k)));
                decisions.AddRange(GetDecisions(board.GetColumn(k)));
            }
            return decisions;
        }

        private List<BinaryDecision> GetDecisions(Cell[] cells)
        {
            List<BinaryDecision> decisions = new List<BinaryDecision>();
            foreach (Cell cell in cells)
            {
                if (cell.info is Notes)
                {
                    Notes notes = (Notes)cell.info;
                    int value1 = -1;
                    int value2 = -1;
                    bool valid = true;
                    for (int k = 0; k < notes.Values.Length; k++)
                    {
                        if (notes.Values[k])
                        {
                            if (value1 < 0)
                            {
                                value1 = k;
                            }
                            else if (value2 < 0)
                            {
                                value2 = k;
                            }
                            else
                            {
                                valid = false;
                            }
                        }
                    }
                    if (valid && value2 >= 0)
                    {
                        decisions.Add(new BinaryRemainingDecision(cell.Index, value1, value2));
                    }
                }
            }
            for (int k = 0; k < CellInfo.numbers; k++)
            {
                decisions.AddRange(GetDecisions(cells, k));
            }
            return decisions;
        }

        private List<BinaryDecision> GetDecisions(Cell[] cells, int number)
        {
            List<BinaryDecision> decisions = new List<BinaryDecision>();
            int cell1 = -1;
            int cell2 = -1;
            bool valid = true;
            foreach (Cell cell in cells)
            {
                if (cell.info is Notes && ((Notes)cell.info).Values[number])
                {
                    if (cell1 < 0)
                    {
                        cell1 = cell.Index;
                    }
                    else if (cell2 < 0)
                    {
                        cell2 = cell.Index;
                    }
                    else
                    {
                        valid = false;
                    }
                }
            }
            if (valid && cell2 >= 0)
            {
                decisions.Add(new BinaryIsolatedDecision(number, cell1, cell2));
            }
            return decisions;
        }

        private bool TryDecision(Board board, BinaryDecision decision, Difficulty cap)
        {
            Board trueDecision = board.Clone();
            Board falseDecision = board.Clone();
            decision.Decide(trueDecision, true);
            decision.Decide(falseDecision, false);
            trueDecision.solution = trueDecision.Solve(cap);
            falseDecision.solution = falseDecision.Solve(cap);
            trueDecision.Finish();
            falseDecision.Finish();
            BoardDifference trueDifference = new BoardDifference(board, trueDecision);
            BoardDifference falseDifference = new BoardDifference(board, falseDecision);
            BoardDifference merged = BoardDifference.Merge(trueDifference, falseDifference);
            if (merged.HasDifferences())
            {
                merged.Apply(board);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class SuperDifferenceFinderResult : StrategyResult
    {
        public override TextboxInfo Step(Board board, int step)
        {
            return null;
        }
    }

    abstract class BinaryDecision
    {
        public abstract void Decide(Board board, bool input);
    }

    class BinaryRemainingDecision : BinaryDecision
    {
        private int cell;
        private int value1;
        private int value2;

        public BinaryRemainingDecision(int cell, int value1, int value2)
        {
            this.cell = cell;
            this.value1 = value1;
            this.value2 = value2;
        }

        public override void Decide(Board board, bool input)
        {
            board.board[cell].info = new Number(input ? value1 : value2);
        }
    }

    class BinaryIsolatedDecision : BinaryDecision
    {
        private int value;
        private int cell1;
        private int cell2;

        public BinaryIsolatedDecision(int value, int cell1, int cell2)
        {
            this.value = value;
            this.cell1 = cell1;
            this.cell2 = cell2;
        }

        public override void Decide(Board board, bool input)
        {
            board.board[input ? cell1 : cell2].info = new Number(value);
        }
    }
}
