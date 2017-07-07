using System;
using System.IO;
using System.Reflection;

namespace Sudoku
{
    public class ComputerTextResource : TextResource
    {
        public override TextReader GetStream(string path)
        {
            return new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Sudoku." + path + ".txt"));
        }
    }
}
