using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku.Strategies
{
    public class Ladder : Strategy
    {
        public override Difficulty difficulty => Difficulty.Hard;

        public override StrategyResult Apply(Board board, ref string outInfo)
        {
            bool flag = false;
            if (Apply(board, board.GetRow, cell => cell.Column))
            {
                flag = true;
            }
            if (Apply(board, board.GetColumn, cell => cell.Row))
            {
                flag = true;
            }
            return flag ? new LadderResult() : null;
        }

        private bool Apply(Board board, Func<int, Cell[]> searchGroup, Func<Cell, int> rungID)
        {
            bool flag = false;
            for (int k = 0; k < CellInfo.numbers; k++)
            {
                if (Apply(board, k, searchGroup, rungID))
                {
                    flag = true;
                }
            }
            return flag;
        }

        private bool Apply(Board board, int number, Func<int, Cell[]> searchGroup, Func<Cell, int> rungID)
        {
            bool flag = false;
            for (int k = 0; k < Board.size; k += Board.boxSize)
            {
                for (int i = 0; i < Board.boxSize; i++)
                {
                    for (int j = i + 1; j < Board.boxSize; j++)
                    {
                        if (Apply(board, number, searchGroup(k + i), searchGroup(k + j), rungID))
                        {
                            flag = true;
                        }
                    }
                }
            }
            return flag;
        }

        private bool Apply(Board board, int number, Cell[] group1, Cell[] group2, Func<Cell, int> rungID)
        {
            List<int> rungs = new List<int>();
            bool flag = false;
            foreach (Cell cell in group1)
            {
                if (CellHasNote(cell, number))
                {
                    rungs.Add(rungID(cell));
                    flag = true;
                }
            }
            if (!flag)
            {
                return false;
            }
            flag = false;
            foreach (Cell cell in group2)
            {
                if (CellHasNote(cell, number))
                {
                    if (!rungs.Contains(rungID(cell)))
                    {
                        rungs.Add(rungID(cell));
                    }
                    flag = true;
                }
            }
            if (!flag)
            {
                return false;
            }
            if (rungs.Count != 3)
            {
                return false;
            }
            bool same01 = false;
            bool same02 = false;
            bool same12 = false;
            if (rungs[0] / 3 == rungs[1] / 3)
            {
                same01 = true;
            }
            if (rungs[0] / 3 == rungs[2] / 3)
            {
                same02 = true;
            }
            if (rungs[1] / 3 == rungs[2] / 3)
            {
                same12 = true;
            }
            if (!same01 && !same02 && !same12)
            {
                return false;
            }
            if (same01 && same02 && same12)
            {
                return false;
            }
            //guaranteed only one flag is true now
            int sweepRung = rungs[0];
            if (same01)
            {
                sweepRung = rungs[2];
            }
            else if (same02)
            {
                sweepRung = rungs[1];
            }
            flag = false;
            var cells = board.board.Where(cell =>
            {
                if (group1.Contains(cell) || group2.Contains(cell))
                {
                    return false;
                }
                return rungID(cell) == sweepRung;
            });
            foreach (Cell cell in cells)
            {
                if (CellHasNote(cell, number))
                {
                    ((Notes)cell.info).Values[number] = false;
                    flag = true;
                }
            }
            return flag;
        }

        private bool CellHasNote(Cell cell, int note)
        {
            return cell.info is Notes && ((Notes)cell.info).Values[note];
        }
    }

    public class LadderResult : StrategyResult
    {
        public override TextboxInfo Step(Board board, int step)
        {
            return null;
        }
    }
}
