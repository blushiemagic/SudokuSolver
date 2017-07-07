using System;
using System.Linq;

namespace Sudoku
{
    public class HelpSnapshot
    {
        public readonly CellInfo[] Contents;
        public readonly Color?[] Colors;
        public readonly TextboxInfo textbox;

        public HelpSnapshot(Board board)
        {
            this.Contents = board.board.Select(cell => cell.info.HelpClone()).ToArray();
            this.Colors = board.board.Select(cell => cell.overrideColor).ToArray();
            this.textbox = board.textbox;
        }

        public void CopyTo(Board board)
        {
            board.SetupTransition();
            for (int k = 0; k < Contents.Length; k++)
            {
                board.board[k].info = Contents[k].HelpClone();
                board.board[k].overrideColor = Colors[k];
            }
            board.ChangeTextbox(textbox);
        }
    }
}
