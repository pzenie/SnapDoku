using System.Collections.Generic;

namespace Sudoku_Solver.Models
{
   internal static class PuzzleVerifier
   {
      public static bool VerifyPuzzle(BoardModel board)
      {
         bool verify = true;
         verify &= VerifyGroups(GroupGetter.GetGroups(board));
         verify &= VerifyGroups(GroupGetter.GetVerticals(board));
         verify &= VerifyGroups(GroupGetter.GetHorizontals(board));
         return verify;
      }

      private static bool VerifyGroups(List<List<Cell>> groups)
      {
         foreach (List<Cell> cells in groups)
         {
            HashSet<int> group = new HashSet<int>();
            foreach (Cell cell in cells)
            {
               int value;
               if (int.TryParse(cell.CellValue, out value))
               {
                  if (value > 0 && value <= cells.Count)
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
