using System;
using System.Linq;
using System.Collections.Generic;

namespace Sudoku.Strategies
{
    public class SpikedTwoByTwo : Strategy
    {
		public override Difficulty difficulty => Difficulty.Hard;

        public override StrategyResult Apply(Board board, ref string outInfo)
        {
            bool flag = false;
            if (Apply(board, board.GetRow, cell => cell.Column, cell => cell.Box))
            {
                flag = true;
            }
            if (Apply(board, board.GetColumn, cell => cell.Row, cell => cell.Box))
            {
                flag = true;
            }
            return flag ? new SpikedTwoByTwoResult() : null;
        }

        private bool Apply(Board board, Func<int, Cell[]> searchGroup, Func<Cell, int> searchID, Func<Cell, int> spikeID)
        {
            bool flag = false;
            for (int k = 0; k < CellInfo.numbers; k++)
            {
                if (Apply(board, k, searchGroup, searchID, spikeID))
                {
                    flag = true;
                }
            }
            return flag;
        }

        private bool Apply(Board board, int number, Func<int, Cell[]> searchGroup, Func<Cell, int> searchID, Func<Cell, int> spikeID)
        {
            bool flag = false;
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
                    for (int j = 0; j < Board.size; j++)
                    {
                        if (j == i)
                        {
                            continue;
                        }
                        Cell[] group2 = searchGroup(j);
                        int useSpikeID = -1;
                        valid = true;
                        bool valid2 = false;
                        foreach (Cell cell in group2)
                        {
                            bool hasNote = CellHasNote(cell, number);
                            bool isImportant = searchID(cell) == searchID1 || searchID(cell) == searchID2;
                            if (hasNote != isImportant)
                            {
                                if (useSpikeID < 0)
                                {
                                    useSpikeID = spikeID(cell);
                                }
                                else if (useSpikeID != spikeID(cell))
                                {
                                    valid = false;
                                }
                            }
                            else if (hasNote && isImportant)
                            {
                                valid2 = true;
                            }
                        }
                        if (valid && valid2 && useSpikeID >= 0)
                        {
                            var cells = board.board.Where(cell =>
                                (searchID(cell) == searchID1 || searchID(cell) == searchID2) && spikeID(cell) == useSpikeID
                                && !group1.Contains(cell) && !group2.Contains(cell));
                            foreach (Cell cell in cells)
                            {
                                if (CellHasNote(cell, number))
                                {
                                    ((Notes)cell.info).Values[number] = false;
                                    flag = true;
                                }
                            }
                        }
                    }
                }
            }
            return flag;
        }

        private bool CellHasNote(Cell cell, int note)
        {
            return cell.info is Notes && ((Notes)cell.info).Values[note];
        }
    }

    public class SpikedTwoByTwoResult : StrategyResult
    {
        public override TextboxInfo Step(Board board, int step)
        {
            return null;
        }
    }
}
