using System;
using System.Collections.Generic;

namespace Sudoku_Solver.Models
{
   internal static class PuzzleVerifier
   {
      public static bool VerifyPuzzle(BoardModel board)
      {
         bool verify = true;
         verify &= VerifyGroups(board, GroupGetter.GetGroups(board));
         verify &= VerifyGroups(board, GroupGetter.GetVerticals(board));
         verify &= VerifyGroups(board, GroupGetter.GetHorizontals(board));
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
               int value;
               if (int.TryParse(cell.CellValue, out value))
               {
                  if (value > 0 && value <= board.BoardValues.Count)
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
