using System;
using System.Collections.Generic;
using System.IO;

namespace Sudoku
{
    public abstract class TextResource
    {
        public string solving;
        public string box;
        public string row;
        public string column;
        public string completed;
        public string unsolvable;
        public string tooDifficult;
        public string elimination;
        public string remainingSingle;
        public string isolatedSingle;
        public string[] trap = new string[3];
        public string[] remainingDouble = new string[7];
        public string[] isolatedDouble = new string[8];

        public abstract TextReader GetStream(string path);

        public void LoadResource(string path)
        {
            Dictionary<string, string> entries = new Dictionary<string, string>();
            using (TextReader reader = GetStream(path))
            {
                for (string entry = reader.ReadLine(); entry != null; entry = reader.ReadLine())
                {
                    int split = entry.IndexOf('=');
                    if (split >= 0)
                    {
                        string name = entry.Substring(0, split);
                        string value = entry.Substring(split + 1);
                        entries[name] = value;
                    }
                }
            }
            solving = entries["Solving"];
            box = entries["Box"];
            row = entries["Row"];
            column = entries["Column"];
            completed = entries["Completed"];
            unsolvable = entries["Unsolvable"];
            tooDifficult = entries["TooDifficult"];
            elimination = entries["Elimination"];
            remainingSingle = entries["RemainingSingle"];
            isolatedSingle = entries["IsolatedSingle"];
            for (int k = 0; k < trap.Length; k++)
            {
                trap[k] = entries["Trap" + (k + 1)];
            }
            for (int k = 0; k < remainingDouble.Length; k++)
            {
                remainingDouble[k] = entries["RemainingDouble" + (k + 1)];
            }
            for (int k = 0; k < isolatedDouble.Length; k++)
            {
                isolatedDouble[k] = entries["IsolatedDouble" + (k + 1)];
            }
        }
    }
}
