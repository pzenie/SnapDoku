using System;
using System.Collections.Generic;

namespace Sudoku_Solver.Solver
{
    public static class Solver
    {
        /// <summary>
        /// Prunes the puzzle and backtracks repeatedly until solved.
        /// </summary>
        /// <param name="board">The board to solve</param>
        /// <param name="groups">The groups each cell in the board belongs to</param>
        /// <returns>The solved puzzle or partially solved if failed</returns>
        public static int[][] PuzzleSolver(int[][] board, List<List<List<Tuple<int, int>>>> groups)
        {
            board = Pruner.PrunePuzzle(board, groups);
            var possibleValues = Pruner.PruneAllCells(board, Pruner.InitPossibleValues(board), groups);
            var location = Backtracker.SkipFilled(board, new Tuple<int, int>(0, 0));
            board = Backtracker.BackTrack(board, possibleValues, location, groups);
            return board;
        }
    }
}
