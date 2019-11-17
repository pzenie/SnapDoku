using Sudoku_Solver.Data;
using System;
using System.Collections.Generic;

namespace Sudoku_Solver.Solver
{
   internal static class Backtracker
   {
      public static BoardModel BackTrack(BoardModel board, List<List<List<Tuple<int,int>>>> groups)
      {
         Stack<BoardModel> boardStack = new Stack<BoardModel>();
         boardStack.Push(new BoardModel(board));
         while (boardStack.Count > 0)
         {
            BoardModel currentBoard = boardStack.Pop();
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
               Pruner.PrunePuzzle(currentBoard, groups);
               PushNextBoards(boardStack, currentBoard, currentCell, nextX, nextY);
            }
         }
         return board;
      }

      private static void PushNextBoards(Stack<BoardModel> boardStack, BoardModel currentBoard, Cell currentCell, int nextX, int nextY)
      {
         SkipFilled(currentBoard, currentCell, nextX, nextY);
         if (currentCell.CellValue.Length != 0)
         {
            boardStack.Push(currentBoard);
         }
         else
         {
            if (AllCellsHavePossibleValues(currentBoard))
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
      }

      private static bool AllCellsHavePossibleValues(BoardModel board)
      {
         for (int i = 0; i < board.BoardValues.Count; i++)
         {
            for (int j = 0; j < board.BoardValues[i].Count; j++)
            {
               Cell currentCell = board.BoardValues[i][j];
               if (currentCell.CellValue.Length == 0 && currentCell.GetPossibleValues().Count == 0)
               {
                  return false;
               }
            }
         }
         return true;
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
               currentBoard.currentX = nextX;
               currentBoard.currentY = nextY;
            }
            else
            {
               break;
            }
         }
      }
      private static Tuple<int, int> GetNextCoordinates(int x, int y, int size)
      {
         if (y + 1 < size)
         {
            return new Tuple<int, int>(x, y + 1);
         }
         else if (x + 1 < size)
         {
            return new Tuple<int, int>(x + 1, 0);
         }
         else
         {
            return new Tuple<int, int>(-1, -1);
         }
      }
   }
}
