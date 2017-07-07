using System;

namespace Sudoku.Strategies
{
    public enum Group
    {
        Box,
        Row,
        Column
    }

    public static class GroupExtensions
    {
        public static string ToText(this Group group)
        {
            if (group == Group.Box)
            {
                return Main.textResource.box;
            }
            else if (group == Group.Row)
            {
                return Main.textResource.row;
            }
            else if (group == Group.Column)
            {
                return Main.textResource.column;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
