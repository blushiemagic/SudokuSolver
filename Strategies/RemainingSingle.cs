using System;
using System.Collections.Generic;

namespace Sudoku.Strategies
{
    public class RemainingSingle : Strategy
    {
		public override Difficulty difficulty => Difficulty.Basic;

        public override StrategyResult Apply(Board board, ref string outInfo)
        {
            RemainingSingleResult result = new RemainingSingleResult();
            foreach (Cell cell in board.board)
            {
                if (Apply(cell))
                {
                    result.cells.Add(cell.Index);
                }
            }
            return result.cells.Count > 0 ? result : null;
        }

        private bool Apply(Cell cell)
        {
            if (cell.info is Notes)
            {
                Notes notes = (Notes)cell.info;
                int number = -1;
                for (int k = 0; k < notes.Values.Length; k++)
                {
                    if (notes.Values[k])
                    {
                        if (number < 0)
                        {
                            number = k;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                if (number >= 0)
                {
                    cell.info = new Number(number);
                    return true;
                }
            }
            return false;
        }
    }

    public class RemainingSingleResult : StrategyResult
    {
        public List<int> cells;

        public RemainingSingleResult()
        {
            this.cells = new List<int>();
        }

        public override TextboxInfo Step(Board board, int step)
        {
            if (step == 1)
            {
                foreach (int cell in cells)
                {
                    board.board[cell].overrideColor = Color.Green;
                }
                return new TextboxInfo(Main.textResource.remainingSingle);
            }
            else
            {
                return null;
            }
        }
    }
}
