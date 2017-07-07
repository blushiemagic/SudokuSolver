using System;
using System.Linq;

namespace Sudoku
{
    public class Snapshot
    {
        public readonly CellInfo[] Contents;

        public Snapshot(CellInfo[] contents)
        {
            this.Contents = contents;
        }

        public Snapshot(Board board)
        {
            this.Contents = board.board.Select(cell => cell.info.Clone()).ToArray();
        }

        public void CopyTo(Board board)
        {
            for (int k = 0; k < Contents.Length; k++)
            {
                board.board[k].info = Contents[k].Clone();
            }
        }
    }
}
