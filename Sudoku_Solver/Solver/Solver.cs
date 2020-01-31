using Sudoku_Solver.Data;
using System;
using System.Collections.Generic;

namespace Sudoku_Solver.Solver
{
    public static class Solver
    {
        public static int[][] PuzzleSolver(int[][] board, List<List<List<Tuple<int, int>>>> groups)
        {
            board = Pruner.PrunePuzzle(board, groups);
            //board = Backtracker.BackTrack(board, groups);
            return board;
        }
    }
}
