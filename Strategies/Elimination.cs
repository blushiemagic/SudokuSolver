using System;
using System.Collections.Generic;

namespace Sudoku.Strategies
{
    public class Elimination : Strategy
    {
		public override Difficulty difficulty => Difficulty.Basic;

		public override StrategyResult Apply(Board board, ref string outInfo)
        {
            EliminationResult result = new EliminationResult();
            bool flag = false;
            for (int k = 0; k < Board.size; k++)
            {
                if (Apply(board.GetBox(k), result))
                {
                    flag = true;
                }
                if (Apply(board.GetRow(k), result))
                {
                    flag = true;
                }
                if (Apply(board.GetColumn(k), result))
                {
                    flag = true;
                }
            }
            return flag ? result : null;
        }

        private bool Apply(Cell[] cells, EliminationResult result)
        {
            List<int> numbers = new List<int>();
            foreach (Cell cell in cells)
            {
                if (cell.info is Number)
                {
                    numbers.Add(((Number)cell.info).Value);
                }
            }
            bool flag = false;
            foreach (Cell cell in cells)
            {
                if (cell.info is Notes)
                {
                    foreach (int number in numbers)
                    {
                        Notes notes = (Notes)cell.info;
                        if (notes.Values[number])
                        {
                            notes.Values[number] = false;
                            result.removedNotes[number].Add(cell.Index);
                            flag = true;
                        }
                    }
                }
            }
            return flag;
        }
    }

    public class EliminationResult : StrategyResult
    {
        public List<int>[] removedNotes;

        public EliminationResult()
        {
            this.removedNotes = new List<int>[CellInfo.numbers];
            for (int k = 0; k < CellInfo.numbers; k++)
            {
                this.removedNotes[k] = new List<int>();
            }
        }

        public override TextboxInfo Step(Board board, int step)
        {
            int number;
            int previousNumber = -1;
            for (number = 0; number < removedNotes.Length; number++)
            {
                if (removedNotes[number].Count > 0)
                {
                    step--;
                    if (step <= 0)
                    {
                        break;
                    }
                    previousNumber = number;
                }
            }
            if (number >= removedNotes.Length)
            {
                return null;
            }
            List<int> rows = new List<int>();
            List<int> columns = new List<int>();
            List<int> boxes = new List<int>();
            foreach (Cell cell in board.board)
            {
                if (cell.info is Number && ((Number)cell.info).Value == number)
                {
                    rows.Add(cell.Row);
                    columns.Add(cell.Column);
                    boxes.Add(cell.Box);
                }
            }
            foreach (Cell cell in board.board)
            {
                if (cell.info is Number && ((Number)cell.info).Value == number)
                {
                    cell.overrideColor = Color.Blue;
                }
                else if (rows.Contains(cell.Row) || columns.Contains(cell.Column) || boxes.Contains(cell.Box))
                {
                    cell.overrideColor = Color.Orange;
                }
            }
            foreach (int cellID in removedNotes[number])
            {
                ((Notes)board.board[cellID].info).OverrideColors[number] = Color.Red;
            }
            if (previousNumber >= 0)
            {
                foreach (int cellID in removedNotes[previousNumber])
                {
                    ((Notes)board.board[cellID].info).Values[previousNumber] = false;
                }
            }
            return new TextboxInfo(Main.textResource.elimination, (number + 1).ToString());
        }
    }
}
