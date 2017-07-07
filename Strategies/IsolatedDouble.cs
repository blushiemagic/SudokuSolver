using System;
using System.Linq;

namespace Sudoku.Strategies
{
    public class IsolatedDouble : Strategy
    {
		public override Difficulty difficulty => Difficulty.Medium;

        public override StrategyResult Apply(Board board, ref string outInfo)
        {
            for (int k = 0; k < Board.size; k++)
            {
                IsolatedDoubleResult result = Apply(board.GetBox(k), Group.Box);
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

        private IsolatedDoubleResult Apply(Cell[] cells, Group groupType)
        {
            for (int i = 0; i < CellInfo.numbers; i++)
            {
                Cell cell1 = null;
                Cell cell2 = null;
                bool valid = true;
                foreach (Cell cell in cells)
                {
                    if (CellHasNote(cell, i))
                    {
                        if (cell1 == null)
                        {
                            cell1 = cell;
                        }
                        else if (cell2 == null)
                        {
                            cell2 = cell;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                }
                if (cell2 == null || !valid)
                {
                    continue;
                }
                for (int j = i + 1; j < CellInfo.numbers; j++)
                {
                    valid = true;
                    foreach (Cell cell in cells)
                    {
                        if ((cell == cell1 || cell == cell2) != CellHasNote(cell, j))
                        {
                            valid = false;
                        }
                    }
                    if (valid)
                    {
                        bool flag = false;
                        IsolatedDoubleResult result = new IsolatedDoubleResult(cells, cell1, cell2, i, j, groupType);
                        for (int k = 0; k < CellInfo.numbers; k++)
                        {
                            if (k != i && k != j)
                            {
                                if (CellHasNote(cell1, k))
                                {
                                    ((Notes)cell1.info).Values[k] = false;
                                    flag = true;
                                }
                                if (CellHasNote(cell2, k))
                                {
                                    ((Notes)cell2.info).Values[k] = false;
                                    flag = true;
                                }
                            }
                        }
                        if (flag)
                        {
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        private bool CellHasNote(Cell cell, int number)
        {
            return cell.info is Notes && ((Notes)cell.info).Values[number];
        }
    }

    public class IsolatedDoubleResult : StrategyResult
    {
        private int[] group;
        private int cell1;
        private Notes info1;
        private int cell2;
        private Notes info2;
        private int number1;
        private int number2;
        private Group groupType;

        public IsolatedDoubleResult(Cell[] group, Cell cell1, Cell cell2, int number1, int number2, Group groupType)
        {
            this.group = group.Select(cell => cell.Index).ToArray();
            this.cell1 = cell1.Index;
            this.info1 = (Notes)cell1.info.Clone();
            this.cell2 = cell2.Index;
            this.info2 = (Notes)cell2.info.Clone();
            this.number1 = number1;
            this.number2 = number2;
            this.groupType = groupType;
        }

        public override TextboxInfo Step(Board board, int step)
        {
            if (step > 8)
            {
                return null;
            }
            foreach (int cell in group)
            {
                board.board[cell].overrideColor = Color.Blue;
            }
            board.board[cell1].overrideColor = Color.Orange;
            board.board[cell2].overrideColor = Color.Orange;
            if (step == 3)
            {
                board.board[cell1].info = new Number(number1);
            }
            else if (step == 4)
            {
                board.board[cell2].info = new Number(number2);
            }
            else if (step == 5 || step == 8)
            {
                board.board[cell1].info = info1.Clone();
                board.board[cell2].info = info2.Clone();
            }
            else if (step == 6)
            {
                board.board[cell2].info = new Number(number1);
            }
            else if (step == 7)
            {
                board.board[cell1].info = new Number(number2);
            }
            SetNoteColor(board.board[cell1].info, number1, Color.Cyan);
            SetNoteColor(board.board[cell1].info, number2, Color.Cyan);
            SetNoteColor(board.board[cell2].info, number1, Color.Cyan);
            SetNoteColor(board.board[cell2].info, number2, Color.Cyan);
            if (step == 3)
            {
                SetNoteColor(board.board[cell2].info, number1, Color.Red);
            }
            else if (step == 6)
            {
                SetNoteColor(board.board[cell1].info, number2, Color.Red);
            }
            else if (step == 8)
            {
                for (int k = 0; k < CellInfo.numbers; k++)
                {
                    if (k != number1 && k != number2)
                    {
                        SetNoteColor(board.board[cell1].info, k, Color.Red);
                        SetNoteColor(board.board[cell2].info, k, Color.Red);
                    }
                }
            }
            return new TextboxInfo(Main.textResource.isolatedDouble[step - 1],
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
