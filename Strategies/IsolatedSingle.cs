using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Strategies
{
    public class IsolatedSingle : Strategy
    {
		public override Difficulty difficulty => Difficulty.Basic;

		public override StrategyResult Apply(Board board, ref string outInfo)
        {
            IsolatedSingleResult result = new IsolatedSingleResult();
            bool flag = false;
            for (int k = 0; k < Board.size; k++)
            {
                if (Apply(board.GetBox(k), Group.Box, result))
                {
                    flag = true;
                }
                if (Apply(board.GetRow(k), Group.Row, result))
                {
                    flag = true;
                }
                if (Apply(board.GetColumn(k), Group.Column, result))
                {
                    flag = true;
                }
            }
            return flag ? result : null;
        }

        private bool Apply(Cell[] cells, Group group, IsolatedSingleResult result)
        {
            bool flag = false;
            for (int k = 0; k < CellInfo.numbers; k++)
            {
                if (Apply(cells, k, group, result))
                {
                    flag = true;
                }
            }
            return flag;
        }

        private bool Apply(Cell[] cells, int number, Group group, IsolatedSingleResult result)
        {
            Cell isolated = null;
            foreach (Cell cell in cells)
            {
                if (cell.info is Number && ((Number)cell.info).Value == number)
                {
                    return false;
                }
                if (cell.info is Notes && ((Notes)cell.info).Values[number])
                {
                    if (isolated == null)
                    {
                        isolated = cell;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (isolated == null)
            {
                return false;
            }
            isolated.info = new Number(number);
            result.AddEntry(isolated.Index, number, group, cells.Select(cell => cell.Index).ToArray());
            return true;
        }
    }

    public class IsolatedSingleResult : StrategyResult
    {
        private List<int> cells;
        private List<int> numbers;
        private List<Group> groupTypes;
        private List<int[]> groups;

        public IsolatedSingleResult()
        {
            this.cells = new List<int>();
            this.numbers = new List<int>();
            this.groupTypes = new List<Group>();
            this.groups = new List<int[]>();
        }

        public void AddEntry(int cell, int number, Group groupType, int[] group)
        {
            cells.Add(cell);
            numbers.Add(number);
            groupTypes.Add(groupType);
            groups.Add(group);
        }

        public override TextboxInfo Step(Board board, int step)
        {
            step--;
            if (step >= cells.Count)
            {
                return null;
            }
            if (step > 0)
            {
                board.board[cells[step - 1]].info = new Number(numbers[step - 1]);
            }
            foreach (int cell in groups[step])
            {
                board.board[cell].overrideColor = Color.Orange;
            }
            board.board[cells[step]].overrideColor = Color.Green;
            ((Notes)board.board[cells[step]].info).OverrideColors[numbers[step]] = Color.Cyan;
            return new TextboxInfo(Main.textResource.isolatedSingle,
                (numbers[step] + 1).ToString(), groupTypes[step].ToText());
        }
    }
}
