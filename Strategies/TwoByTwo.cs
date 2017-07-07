using System;
using System.Linq;
using System.Collections.Generic;

namespace Sudoku.Strategies
{
    public class TwoByTwo : Strategy
    {
		public override Difficulty difficulty => Difficulty.Medium;

        public override StrategyResult Apply(Board board, ref string outInfo)
        {
            TwoByTwoResult result = Apply(board, board.GetRow, cell => cell.Column, Group.Row, Group.Column);
            if (result != null)
            {
                return result;
            }
            result = Apply(board, board.GetColumn, cell => cell.Row, Group.Column, Group.Row);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        private TwoByTwoResult Apply(Board board, Func<int, Cell[]> searchGroup, Func<Cell, int> searchID,
            Group isolateType, Group eliminateType)
        {
            for (int k = 0; k < CellInfo.numbers; k++)
            {
                TwoByTwoResult result = Apply(board, k, searchGroup, searchID, isolateType, eliminateType);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private TwoByTwoResult Apply(Board board, int number, Func<int, Cell[]> searchGroup, Func<Cell, int> searchID,
            Group isolateType, Group eliminateType)
        {
            for (int i = 0; i < Board.size; i++)
            {
                Cell[] group1 = searchGroup(i);
                int searchID1 = -1;
                int searchID2 = -1;
                bool valid = true;
                foreach (Cell cell in group1)
                {
                    if (CellHasNote(cell, number))
                    {
                        if (searchID1 < 0)
                        {
                            searchID1 = searchID(cell);
                        }
                        else if (searchID2 < 0)
                        {
                            searchID2 = searchID(cell);
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
                if (searchID2 >= 0 && valid)
                {
                    for (int j = i + 1; j < Board.size; j++)
                    {
                        Cell[] group2 = searchGroup(j);
                        valid = true;
                        foreach (Cell cell in group2)
                        {
                            bool isImportant = searchID(cell) == searchID1 || searchID(cell) == searchID2;
                            if (CellHasNote(cell, number) != isImportant)
                            {
                                valid = false;
                            }
                        }
                        if (valid)
                        {
                            var cells = board.board.Where(cell =>
                                (searchID(cell) == searchID1 || searchID(cell) == searchID2)
                                && !group1.Contains(cell) && !group2.Contains(cell));
                            bool flag = false;
                            foreach (Cell cell in cells)
                            {
                                if (CellHasNote(cell, number))
                                {
                                    ((Notes)cell.info).Values[number] = false;
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                return new TwoByTwoResult(group1.Select(cell => cell.Index).ToArray(),
                                    group2.Select(cell => cell.Index).ToArray(),
                                    board.board.Where(cell => searchID(cell) == searchID1).Select(cell => cell.Index).ToArray(),
                                    board.board.Where(cell => searchID(cell) == searchID2).Select(cell => cell.Index).ToArray(),
                                    number, isolateType, eliminateType);
                            }
                        }
                    }
                }
            }
            return null;
        }

        private bool CellHasNote(Cell cell, int note)
        {
            return cell.info is Notes && ((Notes)cell.info).Values[note];
        }
    }

    public class TwoByTwoResult : StrategyResult
    {
        private int[] isolate1;
        private int[] isolate2;
        private int[] eliminate1;
        private int[] eliminate2;
        private int number;
        private Group isolateType;
        private Group eliminateType;

        public TwoByTwoResult(int[] isolate1, int[] isolate2, int[] eliminate1, int[] eliminate2, int number,
            Group isolateType, Group eliminateType)
        {
            this.isolate1 = isolate1;
            this.isolate2 = isolate2;
            this.eliminate1 = eliminate1;
            this.eliminate2 = eliminate2;
            this.number = number;
            this.isolateType = isolateType;
            this.eliminateType = eliminateType;
        }

        public override TextboxInfo Step(Board board, int step)
        {
            return null;
        }
    }
}
