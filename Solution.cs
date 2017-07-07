using System;
using System.Collections.Generic;

namespace Sudoku
{
    public class Solution
    {
        public List<Snapshot> snapshots = new List<Snapshot>();
        public List<StrategyResult> steps = new List<StrategyResult>();
        public readonly bool Completed;
        public readonly bool Invalid;

        public Solution(List<Snapshot> snapshots, List<StrategyResult> steps, bool completed, bool invalid)
        {
            if (snapshots.Count != steps.Count + 1)
            {
                throw new ArgumentException("Snapshots must be 1 longer than steps");
            }
            this.snapshots = snapshots;
            this.steps = steps;
            this.Completed = completed;
            this.Invalid = invalid;
        }
    }
}
