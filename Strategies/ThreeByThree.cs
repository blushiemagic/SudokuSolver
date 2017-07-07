using System;
using System.Linq;
using System.Collections.Generic;

namespace Sudoku.Strategies
{
    public class ThreeByThree : Strategy
    {
		public override Difficulty difficulty => Difficulty.Medium;

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
            return flag ? new ThreeByThreeResult() : null;
        }

        private bool Apply(Board board, Func<int, Cell[]> searchGroup, Func<Cell, int> searchID)
        {
            bool flag = false;
            for (int k = 0; k < CellInfo.numbers; k++)
            {
                if (Apply(board, k, searchGroup, searchID))
                {
                    flag = true;
                }
            }
            return flag;
        }

        private bool Apply(Board board, int number, Func<int, Cell[]> searchGroup, Func<Cell, int> searchID)
        {
            bool flag = false;
            for (int i = 0; i < Board.size; i++)
            {
                Cell[] group1 = searchGroup(i);
                SearchIDs searchIDs1 = SearchIDs.Create();
                bool valid = true;
                foreach (Cell cell in group1)
                {
                    if (CellHasNote(cell, number))
                    {
                        if (!searchIDs1.Fill(searchID(cell)))
                        {
                            valid = false;
                        }
                    }
                }
                if (searchIDs1.Value1 < 0 || !valid)
                {
                    continue;
                }
                for (int j = i + 1; j < Board.size; j++)
                {
                    Cell[] group2 = searchGroup(j);
                    SearchIDs searchIDs2 = searchIDs1;
                    valid = true;
                    bool valid2 = false;
                    foreach (Cell cell in group2)
                    {
                        if (CellHasNote(cell, number))
                        {
                            if (!searchIDs2.Fill(searchID(cell)))
                            {
                                valid = false;
                            }
                            valid2 = true;
                        }
                    }
                    if (!valid || !valid2)
                    {
                        continue;
                    }
                    for (int k = j + 1; k < Board.size; k++)
                    {
                        Cell[] group3 = searchGroup(k);
                        SearchIDs searchIDs3 = searchIDs2;
                        valid = true;
                        valid2 = false;
                        foreach (Cell cell in group3)
                        {
                            if (CellHasNote(cell, number))
                            {
                                if (!searchIDs3.Fill(searchID(cell)))
                                {
                                    valid = false;
                                }
                                valid2 = true;
                            }
                        }
                        if (!valid || !valid2 || searchIDs3.Value3 < 0)
                        {
                            continue;
                        }
                        var cells = board.board.Where(cell =>
                        {
                            int cellID = searchID(cell);
                            if (cellID != searchIDs3.Value1 && cellID != searchIDs3.Value2 && cellID != searchIDs3.Value3)
                            {
                                return false;
                            }
                            return !group1.Contains(cell) && !group2.Contains(cell) && !group3.Contains(cell);
                        });
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
            return flag;
        }

        private bool CellHasNote(Cell cell, int note)
        {
            return cell.info is Notes && ((Notes)cell.info).Values[note];
        }
    }

    public class ThreeByThreeResult : StrategyResult
    {
        public override TextboxInfo Step(Board board, int step)
        {
            return null;
        }
    }
}
