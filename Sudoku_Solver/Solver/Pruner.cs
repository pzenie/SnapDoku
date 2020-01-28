using Sudoku_Solver.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku_Solver.Solver
{
   internal static class Pruner
   {
      public static void PrunePuzzle(BoardModel board, List<List<List<Tuple<int, int>>>> groups)
      {
         bool changed = true;
         PruneAllCells(board, groups);
         while (changed)
         {
            changed = false;
            foreach(var group in groups)
            {
               if(AssignForcedCells(board, group))
               {
                  changed = true;
                  PruneAllCells(board, groups);
               }
            }
         }
      }

      private static void PruneAllCells(BoardModel board, List<List<List<Tuple<int, int>>>> groups)
      {
         foreach (Cell[] row in board.BoardValues)
         {
            foreach (Cell cell in row)
            {
               if (cell.CellValue.Length == 0)
               {
                  foreach (List<List<Tuple<int, int>>> group in groups)
                  {
                     PruneCell(board, group, cell);
                  }
               }
               else if (cell.GetPossibleValues().Count > 1)
               {
                  List<string> values = new List<string>();
                  foreach (string s in cell.GetPossibleValues())
                  {
                     values.Add(s);
                  }
                  foreach (string val in values)
                  {
                     if (val != cell.CellValue)
                     {
                        cell.RemovePossibleValue(val);
                     }
                  }
               }
            }
         }
      }

      private static void PruneCell(BoardModel board, List<List<Tuple<int,int>>> group, Cell cell)
      {
         foreach (List<Tuple<int,int>> grouping in group)
         {
            var location = new Tuple<int, int>(cell.x, cell.y);
            if (grouping.Contains(location))
            {
               foreach (Tuple<int,int> cellLocation in grouping)
               {
                  Cell tempCell = board.BoardValues[cellLocation.Item1][cellLocation.Item2];
                  if (cell != tempCell && tempCell.CellValue.Length != 0)
                  {
                     cell.RemovePossibleValue(tempCell.CellValue);
                  }
               }
               break;
            }
         }
      }

      private static bool AssignForcedCells(BoardModel board, List<List<Tuple<int,int>>> group)
      {
         bool changed = false;
         foreach (List<Tuple<int,int>> grouping in group)
         {
            if(changed)
            {
               break;
            }
            foreach (Tuple<int, int> cellLocation in grouping)
            {
               if(changed)
               {
                  break;
               }
               Cell cell = board.BoardValues[cellLocation.Item1][cellLocation.Item2];
               if (cell.CellValue.Length == 0)
               {
                  var possibleValues = cell.GetPossibleValues();
                  if (possibleValues.Count == 1)
                  {
                     cell.CellValue = possibleValues.First();
                     changed = true;
                     break;
                  }
                  foreach (string value in possibleValues)
                  {
                     bool unique = true;
                     foreach (Tuple<int, int> innerCellLocation in grouping)
                     {
                        Cell innerCell = board.BoardValues[innerCellLocation.Item1][innerCellLocation.Item2];
                        if (innerCell != cell && (innerCell.CellValue == value || (innerCell.CellValue.Length == 0 && innerCell.GetPossibleValues().Contains(value))))
                        {
                           unique = false;
                           break;
                        }
                     }
                     if (unique)
                     {
                        cell.CellValue = value;
                        changed = true;
                        break;
                     }
                  }
               }
            }
         }
         return changed;
      }
   }
}
