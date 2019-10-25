using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Sudoku_Solver.Models
{
   internal static class Solver
   {
      public static BoardModel PuzzleSolver(BoardModel board)
      {
         PrunePuzzle(board);
         board = BackTrack(board);
         return board;
      }

      private static BoardModel BackTrack(BoardModel board)
      {
         Stack<BoardModel> boardStack = new Stack<BoardModel>();
         boardStack.Push(new BoardModel(board));
         Stopwatch sw = new Stopwatch();
         sw.Start();
         while (boardStack.Count > 0)//&& sw.ElapsedMilliseconds < 10000)
         {
            BoardModel currentBoard = boardStack.Pop();
           // board = currentBoard;
            Tuple<int, int> coords = GetNextCoordinates(currentBoard.currentX, currentBoard.currentY, currentBoard.BoardValues.Count);
            int nextX = coords.Item1;
            int nextY = coords.Item2;
            Cell currentCell = currentBoard.BoardValues[currentBoard.currentX][currentBoard.currentY];
            if (nextX == -1 || nextY == -1) // End of board
            {
               if (currentCell.CellValue.Length == 0)
               {
                  if (currentCell.GetPossibleValues().Count == 1) // should only be one option if valid, 0 if not
                  {
                     currentCell.CellValue = currentCell.GetPossibleValues()[0];
                     return currentBoard;
                  }
               }
               else
               {
                  return currentBoard;
               }
            }
            else
            {
               PrunePuzzle(currentBoard);
               PushNextBoards(boardStack, currentBoard, currentCell, nextX, nextY);
            }
         }
         return board;
      }

      private static void PushNextBoards(Stack<BoardModel> boardStack, BoardModel currentBoard, Cell currentCell, int nextX, int nextY)
      {
         //SkipFilled();
         if (currentCell.CellValue.Length != 0)
         {
            currentBoard.currentX = nextX;
            currentBoard.currentY = nextY;
            boardStack.Push(currentBoard);
         }
         else
         {
            foreach (string value in currentCell.GetPossibleValues())
            {
               BoardModel nextBoard = new BoardModel(currentBoard);
               nextBoard.currentX = nextX;
               nextBoard.currentY = nextY;
               nextBoard.BoardValues[currentBoard.currentX][currentBoard.currentY].CellValue = value;
               nextBoard.BoardValues[currentBoard.currentX][currentBoard.currentY].RemovePossibleValue(value);
               boardStack.Push(nextBoard);
            }
         }
      }

      private static void SkipFilled(BoardModel currentBoard, Cell currentCell, int nextX, int nextY)
      {
         while (currentCell.CellValue.Length != 0)
         {
            Tuple<int, int> res = GetNextCoordinates(nextX, nextY, currentBoard.BoardValues.Count);
            nextX = res.Item1;
            nextY = res.Item2;
            if (nextX > -1 && nextY > -1)
            {
               currentCell = currentBoard.BoardValues[nextX][nextY];
            }
            else
            {
               break;
            }
         }
      }
      private static Tuple<int,int> GetNextCoordinates(int x, int y, int size)
      {
         if(y+1 < size)
         {
            return new Tuple<int, int>(x, y+1);
         }
         else if (x+1 < size)
         {
            return new Tuple<int, int>(x+1, 0);
         }
         else
         {
            return new Tuple<int, int>(-1, -1);
         }
      }

      private static void PrunePuzzle(BoardModel board)
      {
         bool changed = true;
         List<List<Cell>> verticals = GroupGetter.GetVerticals(board);
         List<List<Cell>> horizontals = GroupGetter.GetHorizontals(board);
         List<List<Cell>> groups = GroupGetter.GetGroups(board);
         PruneAllCells(board, verticals, horizontals, groups);
         //while (changed)
         //{
            //AssignAndPrune(verticals, board, verticals, horizontals, groups);
            //changed = AssignAndPrune(horizontals, board, verticals, horizontals, groups);
            //changed |= AssignAndPrune(groups, board, verticals, horizontals, groups);
         //}
      }

      private static bool AssignAndPrune(List<List<Cell>> group, BoardModel board, List<List<Cell>> verticals, List<List<Cell>> horizontals, List<List<Cell>> groups)
      {
         if (AssignForcedCells(group))
         {
            PruneAllCells(board, verticals, horizontals, groups);
            return true;
         }
         return false;
      }

      private static void PruneAllCells(BoardModel board, List<List<Cell>> verticals, List<List<Cell>> horizontals, List<List<Cell>> groups)
      {
         foreach (ObservableCollection<Cell> row in board.BoardValues)
         {
            foreach (Cell cell in row)
            {
               if (cell.CellValue.Length == 0)
               {
                  PruneCell(verticals, cell);
                  PruneCell(horizontals, cell);
                  PruneCell(groups, cell);
               }
               else if (cell.GetPossibleValues().Count > 1)
               {
                  List<string> values = new List<string>();
                  foreach(string s in cell.GetPossibleValues())
                  {
                     values.Add(s);
                  }
                  foreach (string val in values)
                  {
                     if(val != cell.CellValue)
                     {
                        cell.RemovePossibleValue(val);
                     }
                  }
               }
            }
         }
      }

      private static void PruneCell(List<List<Cell>> group, Cell cell)
      {
         foreach (List<Cell> grouping in group)
         {
            if (grouping.Contains(cell))
            {
               foreach (Cell tempCell in grouping)
               {
                  if (cell != tempCell && tempCell.CellValue.Length != 0)
                  {
                     cell.RemovePossibleValue(tempCell.CellValue);
                  }
               }
               if (cell.GetPossibleValues().Count == 1)
               {
                  //cell.CellValue = cell.GetPossibleValues()[0];
               }
               break;
            }
         }
      }

      private static bool AssignForcedCells(List<List<Cell>> group)
      {
         bool changed = false;
         foreach (List<Cell> grouping in group)
         {
            foreach (Cell cell in grouping)
            {
               if (string.IsNullOrWhiteSpace(cell.CellValue))
               {
                  foreach (string value in cell.GetPossibleValues())
                  {
                     bool unique = true;
                     foreach (Cell innerCell in grouping)
                     {
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
