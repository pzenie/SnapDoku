using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Sudoku_Solver.Models
{
   internal static class Solver
   {
      public static BoardModel PuzzleSolver(BoardModel board, List<List<List<Tuple<int,int>>>> groups)
      {
         Pruner.PrunePuzzle(board, groups);
         board = Backtracker.BackTrack(board, groups);
         return board;
      }
   }
}
