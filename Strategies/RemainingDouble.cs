using System;
using System.Linq;

namespace Sudoku.Strategies
{
    public class RemainingDouble : Strategy
    {
		public override Difficulty difficulty => Difficulty.Medium;

        public override StrategyResult Apply(Board board, ref string outInfo)
        {
            for (int k = 0; k < Board.size; k++)
            {
                RemainingDoubleResult result = Apply(board.GetBox(k), Group.Box);
                if (result != null)
                {
                    return result;
                }
                result = Apply(board.GetRow(k), Group.Row);
                if (result != null)
                {
                    return result;
                }
                result = Apply(board.GetColumn(k), Group.Column);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private RemainingDoubleResult Apply(Cell[] cells, Group groupType)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                if (!(cells[i].info is Notes))
                {
                    continue;
                }
                Notes notes = (Notes)cells[i].info;
                int value1 = -1;
                int value2 = -1;
                bool valid = true;
                for (int k = 0; k < notes.Values.Length; k++)
                {
                    if (notes.Values[k])
                    {
                        if (value1 < 0)
                        {
                            value1 = k;
                        }
                        else if (value2 < 0)
                        {
                            value2 = k;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
                if (value2 < 0 || !valid)
                {
                    continue;
                }
                for (int j = i + 1; j < cells.Length; j++)
                {
                    if (notes.Equals(cells[j].info))
                    {
                        bool flag = false;
                        for (int k = 0; k < cells.Length; k++)
                        {
                            if (k != i && k != j && cells[k].info is Notes)
                            {
                                Notes target = (Notes)cells[k].info;
                                if (target.Values[value1] || target.Values[value2])
                                {
                                    target.Values[value1] = false;
                                    target.Values[value2] = false;
                                    flag = true;
                                }
                            }
                        }
                        if (flag)
                        {
                            return new RemainingDoubleResult(cells.Select(cell => cell.Index).ToArray(),
                                cells[i].Index, cells[j].Index, value1, value2, groupType);
                        }
                    }
                }
            }
            return null;
        }
    }

    public class RemainingDoubleResult : StrategyResult
    {
        private int[] group;
        private int cell1;
        private int cell2;
        private int number1;
        private int number2;
        private Group groupType;

        public RemainingDoubleResult(int[] group, int cell1, int cell2, int number1, int number2, Group groupType)
        {
            this.group = group;
            this.cell1 = cell1;
            this.cell2 = cell2;
            this.number1 = number1;
            this.number2 = number2;
            this.groupType = groupType;
        }

        public override TextboxInfo Step(Board board, int step)
        {
            if (step > 7)
            {
                return null;
            }
            foreach (int cell in group)
            {
                board.board[cell].overrideColor = Color.Orange;
            }
            board.board[cell1].overrideColor = Color.Blue;
            board.board[cell2].overrideColor = Color.Blue;
            if (step == 2)
            {
                board.board[cell1].info = new Number(number1);
            }
            else if (step == 3)
            {
                board.board[cell2].info = new Number(number2);
            }
            else if (step == 4 || step == 7)
            {
                Notes notes = new Notes();
                for (int k = 0; k < CellInfo.numbers; k++)
                {
                    notes.Values[k] = false;
                }
                notes.Values[number1] = true;
                notes.Values[number2] = true;
                board.board[cell1].info = notes.Clone();
                board.board[cell2].info = notes.Clone();
            }
            else if (step == 5)
            {
                board.board[cell1].info = new Number(number2);
            }
            else if (step == 6)
            {
                board.board[cell2].info = new Number(number1);
            }
            if (step == 1 || step == 4 || step == 7)
            {
                SetNoteColor(board.board[cell1].info, number1, Color.Cyan);
                SetNoteColor(board.board[cell1].info, number2, Color.Cyan);
                SetNoteColor(board.board[cell2].info, number1, Color.Cyan);
                SetNoteColor(board.board[cell2].info, number2, Color.Cyan);
            }
            if (step == 2 || step == 3 || step == 6)
            {
                foreach (int cell in group)
                {
                    SetNoteColor(board.board[cell].info, number1, Color.Red);
                }
            }
            if (step == 3 || step == 5 || step == 6)
            {
                foreach (int cell in group)
                {
                    SetNoteColor(board.board[cell].info, number2, Color.Red);
                }
            }
            if (step == 7)
            {
                foreach (int cell in group)
                {
                    if (cell != cell1 && cell != cell2)
                    {
                        SetNoteColor(board.board[cell].info, number1, Color.Red);
                        SetNoteColor(board.board[cell].info, number2, Color.Red);
                    }
                }
            }
            return new TextboxInfo(Main.textResource.remainingDouble[step - 1],
                (number1 + 1).ToString(), (number2 + 1).ToString(), groupType.ToText());
        }

        private void SetNoteColor(CellInfo info, int number, Color color)
        {
            if (info is Notes && ((Notes)info).Values[number])
            {
                ((Notes)info).OverrideColors[number] = color;
            }
        }
    }
}
