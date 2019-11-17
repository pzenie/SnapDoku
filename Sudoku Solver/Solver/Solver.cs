using Sudoku_Solver.Data;
using System;
using System.Collections.Generic;

namespace Sudoku_Solver.Solver
{
   public static class Solver
   {
      public static BoardModel PuzzleSolver(BoardModel board, List<List<List<Tuple<int,int>>>> groups)
      {
         Pruner.PrunePuzzle(board, groups);
         board = Backtracker.BackTrack(board, groups);
         return board;
      }
   }
}
