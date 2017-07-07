using System;

namespace Sudoku
{
    public abstract class Strategy
    {
		public abstract Difficulty difficulty
		{
			get;
		}

        public abstract StrategyResult Apply(Board board, ref string outInfo);
    }
}
