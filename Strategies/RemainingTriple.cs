using System;
using System.Collections.Generic;

namespace Sudoku.Strategies
{
    public class RemainingTriple : Strategy
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
            return flag ? new RemainingTripleResult() : null;
        }

        private bool Apply(Cell[] cells)
        {
            bool flag = false;
            for (int i = 0; i < cells.Length; i++)
            {
                if (!(cells[i].info is Notes))
                {
                    continue;
                }
                Notes notes1 = (Notes)cells[i].info;
                List<int> values1 = new List<int>();
                bool valid = true;
                for (int n = 0; n < notes1.Values.Length; n++)
                {
                    if (notes1.Values[n])
                    {
                        if (values1.Count < 3)
                        {
                            values1.Add(n);
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
                if (values1.Count < 2 || !valid)
                {
                    continue;
                }
                for (int j = i + 1; j < cells.Length; j++)
                {
                    if (!(cells[j].info is Notes))
                    {
                        continue;
                    }
                    Notes notes2 = (Notes)cells[j].info;
                    List<int> values2 = new List<int>(values1);
                    valid = true;
                    for (int n = 0; n < notes2.Values.Length; n++)
                    {
                        if (notes2.Values[n])
                        {
                            if (values2.Count < 3)
                            {
                                values2.Add(n);
                            }
                            else
                            {
                                valid = false;
                            }
                        }
                    }
                    if (!valid)
                    {
                        continue;
                    }
                    for (int k = j + 1; k < cells.Length; k++)
                    {
                        if (!(cells[k].info is Notes))
                        {
                            continue;
                        }
                        Notes notes3 = (Notes)cells[k].info;
                        List<int> values3 = new List<int>(values2);
                        valid = true;
                        for (int n = 0; n < notes3.Values.Length; n++)
                        {
                            if (notes3.Values[n])
                            {
                                if (values3.Count < 3)
                                {
                                    values3.Add(n);
                                }
                                else
                                {
                                    valid = false;
                                }
                            }
                        }
                        if (!valid || values3.Count != 3)
                        {
                            continue;
                        }
                        for (int n = 0; n < cells.Length; n++)
                        {
                            if (n != i && n != j && n != k && cells[n].info is Notes)
                            {
                                Notes target = (Notes)cells[n].info;
                                foreach (int value in values3)
                                {
                                    if (target.Values[value])
                                    {
                                        target.Values[value] = false;
                                        flag = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return flag;
        }
    }

    public class RemainingTripleResult : StrategyResult
    {
        public override TextboxInfo Step(Board board, int step)
        {
            return null;
        }
    }
}
