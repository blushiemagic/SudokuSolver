using System;

namespace Sudoku.Strategies
{
    public class BoardDifference
    {
        private RemovedNotes[] removedNotes;

        private BoardDifference()
        {
            this.removedNotes = new RemovedNotes[Board.size * Board.size];
            for (int k = 0; k < Board.size * Board.size; k++)
            {
                this.removedNotes[k] = new RemovedNotes();
            }
        }

        public BoardDifference(Board original, Board newBoard)
        {
            this.removedNotes = new RemovedNotes[Board.size * Board.size];
            for (int k = 0; k < Board.size * Board.size; k++)
            {
                this.removedNotes[k] = new RemovedNotes();
                if (original.board[k].info is Notes)
                {
                    Notes originalNotes = (Notes)original.board[k].info;
                    bool[] newNotes;
                    if (newBoard.board[k].info is Notes)
                    {
                        newNotes = ((Notes)newBoard.board[k].info).Values;
                    }
                    else if (newBoard.board[k].info is Number)
                    {
                        newNotes = new bool[CellInfo.numbers];
                        newNotes[((Number)newBoard.board[k].info).Value] = true;
                    }
                    else
                    {
                        continue;
                    }
                    for (int n = 0; n < CellInfo.numbers; n++)
                    {
                        if (originalNotes.Values[n] && !newNotes[n])
                        {
                            this.removedNotes[k].Remove(n);
                        }
                    }
                }
            }
        }

        public static BoardDifference Merge(BoardDifference first, BoardDifference second)
        {
            BoardDifference merged = new BoardDifference();
            for (int k = 0; k < Board.size * Board.size; k++)
            {
                RemovedNotes mergedNotes = merged.removedNotes[k];
                RemovedNotes firstNotes = first.removedNotes[k];
                RemovedNotes secondNotes = second.removedNotes[k];
                for (int n = 0; n < CellInfo.numbers; n++)
                {
                    if (firstNotes.Removed[n] && secondNotes.Removed[n])
                    {
                        mergedNotes.Remove(n);
                    }
                }
            }
            return merged;
        }

        public bool HasDifferences()
        {
            foreach (RemovedNotes notes in removedNotes)
            {
                foreach (bool removed in notes.Removed)
                {
                    if (removed)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Apply(Board board)
        {
            for (int k = 0; k < Board.size * Board.size; k++)
            {
                removedNotes[k].Apply(board.board[k]);
            }
        }
    }

    public class RemovedNotes
    {
        public readonly bool[] Removed;

        public RemovedNotes()
        {
            this.Removed = new bool[CellInfo.numbers];
        }

        public void Remove(int number)
        {
            this.Removed[number] = true;
        }

        public void Apply(Cell cell)
        {
            if (cell.info is Notes)
            {
                Notes notes = (Notes)cell.info;
                for (int k = 0; k < CellInfo.numbers; k++)
                {
                    if (Removed[k])
                    {
                        notes.Values[k] = false;
                    }
                }
            }
        }
    }
}
