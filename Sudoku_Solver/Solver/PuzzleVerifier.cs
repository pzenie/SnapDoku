using Sudoku_Solver.Data;
using System;
using System.Collections.Generic;

namespace Sudoku_Solver.Solver
{
   public static class PuzzleVerifier
   {
      public static bool VerifyPuzzle(BoardModel board, List<List<List<Tuple<int, int>>>> groups)
      {
         bool verify = true;
         foreach(var group in groups)
         {
            verify &= VerifyGroups(board, group);
         }
         return verify;
      }

      private static bool VerifyGroups(BoardModel board, List<List<Tuple<int,int>>> groups)
      {
         foreach (List<Tuple<int, int>> cellLocations in groups)
         {
            HashSet<int> group = new HashSet<int>();
            foreach (Tuple<int, int> cellLocation in cellLocations)
            {
               Cell cell = board.BoardValues[cellLocation.Item1][cellLocation.Item2];
               if (int.TryParse(cell.CellValue, out int value))
               {
                  if (value > 0 && value <= board.BoardValues.Length)
                  {
                     if (group.Contains(value))
                     {
                        return false;
                     }
                     group.Add(value);
                  }
                  else
                  {
                     return false;
                  }
               }
               else
               {
                  return false;
               }
            }
         }
         return true;
      }
   }
}
