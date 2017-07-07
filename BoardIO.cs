using System.IO;

namespace Sudoku
{
    public partial class Board
    {
        private const string savePath = "save.txt";

        public void Save()
        {
            using (FileStream stream = File.OpenWrite(savePath))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    for (int y = 0; y < size; y++)
                    {
                        for (int x = 0; x < size; x++)
                        {
                            Cell cell = board[x + size * y];
                            if (cell.info is Number)
                            {
                                writer.Write((char)(((Number)cell.info).Value + '1'));
                            }
                            else
                            {
                                writer.Write('?');
                            }
                            if (x == 2 || x == 5)
                            {
                                writer.Write('|');
                            }
                        }
                        writer.WriteLine();
                        if (y == 2 || y == 5)
                        {
                            for (int x = 0; x < size; x++)
                            {
                                writer.Write('-');
                                if (x == 2 || x == 5)
                                {
                                    writer.Write('+');
                                }
                            }
                            writer.WriteLine();
                        }
                    }
                }
            }
        }

        public void Load()
        {
            if (!File.Exists(savePath))
            {
                return;
            }
            using (StreamReader reader = File.OpenText(savePath))
            {
                int index = 0;
                for (int value = reader.Read(); value >= 0; value = reader.Read())
                {
                    if (value == '?')
                    {
                        board[index].info = null;
                        index++;
                    }
                    else if (value >= '1' && value <= '9')
                    {
                        board[index].info = new Number(value - '1', true);
                        index++;
                    }
                }
            }
        }
    }
}
