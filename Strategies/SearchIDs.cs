using System;

namespace Sudoku.Strategies
{
    public struct SearchIDs
    {
        public int Value1;
        public int Value2;
        public int Value3;

        public static SearchIDs Create()
        {
            return new SearchIDs()
            {
                Value1 = -1,
                Value2 = -1,
                Value3 = -1
            };
        }

        public bool Fill(int value)
        {
            if (Contains(value))
            {
                return true;
            }
            if (Value1 < 0)
            {
                Value1 = value;
            }
            else if (Value2 < 0)
            {
                Value2 = value;
            }
            else if (Value3 < 0)
            {
                Value3 = value;
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool Contains(int value)
        {
            return value == Value1 || value == Value2 || value == Value3;
        }
    }
}
