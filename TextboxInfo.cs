using System;

namespace Sudoku
{
    public class TextboxInfo
    {
        public readonly string Text;
        public readonly string[] Arguments;

        public TextboxInfo(string text, params string[] args)
        {
            this.Text = text;
            this.Arguments = args;
        }
    }
}
