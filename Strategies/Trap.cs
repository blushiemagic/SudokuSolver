using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.Strategies
{
    public class Trap : Strategy
    {
		public override Difficulty difficulty => Difficulty.Easy;

        public override StrategyResult Apply(Board board, ref string outInfo)
        {
            TrapResult result = Apply(board, box => board.GetBox(box), cell => cell.Row, Group.Box, Group.Row);
            if (result != null)
            {
                return result;
            }
            result = Apply(board, box => board.GetBox(box), cell => cell.Column, Group.Box, Group.Column);
            if (result != null)
            {
                return result;
            }
            result = Apply(board, box => board.GetRow(box), cell => cell.Box, Group.Row, Group.Box);
            if (result != null)
            {
                return result;
            }
            result = Apply(board, box => board.GetColumn(box), cell => cell.Box, Group.Column, Group.Box);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        private TrapResult Apply(Board board, Func<int, Cell[]> getGroup, Func<Cell, int> trapID,
            Group groupType, Group trapType)
        {
            for (int k = 0; k < Board.size; k++)
            {
                Cell[] group = getGroup(k);
                for (int num = 0; num < CellInfo.numbers; num++)
                {
                    TrapResult result = Apply(board, group, num, trapID, groupType, trapType);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        private TrapResult Apply(Board board, Cell[] group, int number, Func<Cell, int> trapID,
            Group groupType, Group trapType)
        {
            int id = -1;
            foreach (Cell cell in group)
            {
                if (cell.info is Notes && ((Notes)cell.info).Values[number])
                {
                    int cellID = trapID(cell);
                    if (id < 0)
                    {
                        id = cellID;
                    }
                    else if (id != cellID)
                    {
                        return null;
                    }
                }
            }
            if (id < 0)
            {
                return null;
            }
            var targets = board.board.Where(cell => trapID(cell) == id && !group.Contains(cell));
            bool flag = false;
            foreach (Cell cell in targets)
            {
                if (cell.info is Notes)
                {
                    Notes notes = (Notes)cell.info;
                    if (notes.Values[number])
                    {
                        notes.Values[number] = false;
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                return new TrapResult(group.Select(cell => cell.Index).ToArray(),
                    board.board.Where(cell => trapID(cell) == id).Select(cell => cell.Index).ToArray(),
                    number, groupType, trapType);
            }
            return null;
        }
    }

    public class TrapResult : StrategyResult
    {
        private int[] group;
        private int[] trap;
        private int number;
        private Group groupType;
        private Group trapType;

        public TrapResult(int[] group, int[] trap, int number, Group groupType, Group trapType)
        {
            this.group = group;
            this.trap = trap;
            this.number = number;
            this.groupType = groupType;
            this.trapType = trapType;
        }

        public override TextboxInfo Step(Board board, int step)
        {
            if (step <= 3)
            {
                foreach (int cell in group)
                {
                    board.board[cell].overrideColor = Color.Purple;
                }
                foreach (int cell in trap)
                {
                    Color cellColor;
                    Color noteColor;
                    if (group.Contains(cell))
                    {
                        cellColor = Color.Blue;
                        noteColor = Color.Cyan;
                    }
                    else
                    {
                        cellColor = Color.Orange;
                        noteColor = Color.Red;
                        if (step < 3)
                        {
                            noteColor = cellColor;
                        }
                    }
                    board.board[cell].overrideColor = cellColor;
                    if (board.board[cell].info is Notes && ((Notes)board.board[cell].info).Values[number])
                    {
                        ((Notes)board.board[cell].info).OverrideColors[number] = noteColor;
                    }
                }
                return new TextboxInfo(Main.textResource.trap[step - 1],
                    (number + 1).ToString(), groupType.ToText(), trapType.ToText());
            }
            else
            {
                return null;
            }
        }
    }
}
