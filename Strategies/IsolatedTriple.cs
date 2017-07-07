using System;
using System.Collections.Generic;

namespace Sudoku.Strategies
{
    public class IsolatedTriple : Strategy
    {
        public override Difficulty difficulty => Difficulty.Extreme;

        public override StrategyResult Apply(Board board, ref string outInfo)
        {
            bool flag = false;
            for (int k = 0; k < Board.size; k++)
            {
                if (Apply(board.GetBox(k)))
                {
                    flag = true;
                }
                if (Apply(board.GetRow(k)))
                {
                    flag = true;
                }
                if (Apply(board.GetColumn(k)))
                {
                    flag = true;
                }
            }
            return flag ? new IsolatedTripleResult() : null;
        }

        private bool Apply(Cell[] cells)
        {
            bool flag = false;
            for (int i = 0; i < CellInfo.numbers; i++)
            {
                List<Cell> chosenCells1 = new List<Cell>();
                bool valid = true;
                foreach (Cell cell in cells)
                {
                    if (CellHasNote(cell, i))
                    {
                        if (chosenCells1.Count < 3)
                        {
                            chosenCells1.Add(cell);
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
                if (chosenCells1.Count < 2 || !valid)
                {
                    continue;
                }
                for (int j = i + 1; j < CellInfo.numbers; j++)
                {
                    List<Cell> chosenCells2 = new List<Cell>(chosenCells1);
                    valid = true;
                    bool valid2 = false;
                    foreach (Cell cell in cells)
                    {
                        if (CellHasNote(cell, j))
                        {
                            valid2 = true;
                            if (!chosenCells2.Contains(cell))
                            {
                                if (chosenCells2.Count < 3)
                                {
                                    chosenCells2.Add(cell);
                                }
                                else
                                {
                                    valid = false;
                                }
                            }
                        }
                    }
                    if (!valid || !valid2)
                    {
                        continue;
                    }
                    for (int k = j + 1; k < CellInfo.numbers; k++)
                    {
                        List<Cell> chosenCells3 = new List<Cell>(chosenCells2);
                        valid = true;
                        valid2 = false;
                        foreach (Cell cell in cells)
                        {
                            if (CellHasNote(cell, k))
                            {
                                valid2 = true;
                                if (!chosenCells3.Contains(cell))
                                {
                                    if (chosenCells3.Count < 3)
                                    {
                                        chosenCells3.Add(cell);
                                    }
                                    else
                                    {
                                        valid = false;
                                    }
                                }
                            }
                        }
                        if (valid && valid2 && chosenCells3.Count == 3)
                        {
                            for (int n = 0; n < CellInfo.numbers; n++)
                            {
                                if (n != i && n != j && n != k)
                                {
                                    foreach (Cell cell in chosenCells3)
                                    {
                                        if (CellHasNote(cell, n))
                                        {
                                            ((Notes)cell.info).Values[n] = false;
                                            flag = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return flag;
        }

        private bool CellHasNote(Cell cell, int number)
        {
            return cell.info is Notes && ((Notes)cell.info).Values[number];
        }
    }

    public class IsolatedTripleResult : StrategyResult
    {
        public override TextboxInfo Step(Board board, int step)
        {
            return null;
        }
    }
}
