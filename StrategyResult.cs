using System;
using System.Collections.Generic;

namespace Sudoku
{
    public abstract class StrategyResult
    {
        public abstract TextboxInfo Step(Board board, int step);
    }
}
