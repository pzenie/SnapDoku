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
            var possibleValues = Pruner.PruneAllCells(board, Pruner.InitPossibleValues(board), groups);
            var location = Backtracker.SkipFilled(board, new Tuple<int, int>(0, 0));
            board = Backtracker.BackTrack(board, possibleValues, location, groups);
            return board;
        }
    }
}
