using System;

namespace Sudoku
{
    public abstract class Input
    {
        public abstract Point MousePos { get; }
        public bool MouseClick
        {
            get;
            protected set;
        }
        public bool MousePress
        {
            get;
            protected set;
        }
        public bool MouseRelease
        {
            get;
            protected set;
        }

        public abstract void Update();

        public abstract bool KeyType(Control key);

        public abstract bool KeyPress(Control key);

        public abstract bool KeyRelease(Control key);
    }

    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public enum Control
    {
        None,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        D8,
        D9,
        Back,
        Count
    }
}
