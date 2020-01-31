using Sudoku_Solver.Data;
using System;
using System.Collections.Generic;

namespace Sudoku_Solver.Solver
{
    internal static class Backtracker
    {
         public static BoardModel BackTrack(BoardModel board, List<List<List<Tuple<int, int>>>> groups)
         {
             if(board.currentX == -1 && board.currentY == -1)
             {
                 return board;
             }
             Cell currentCell = board.BoardValues[board.currentX][board.currentY];
             if (currentCell.GetPossibleValues().Count == 0) return null;
             List<int> values = new List<int>(currentCell.GetPossibleValues());
             foreach (int value in values)
             {
                 BoardModel tempBoard = new BoardModel(board);
                 tempBoard.BoardValues[tempBoard.currentX][tempBoard.currentY].CellValue = value;
                 //Pruner.PrunePuzzle(tempBoard, groups);
                 var nextCords = GetNextCoordinates(tempBoard.currentX, tempBoard.currentY, tempBoard.BoardValues.Length);
                 int x = nextCords.Item1;
                 int y = nextCords.Item2;
                 SkipFilled(tempBoard, currentCell, x, y);
                 tempBoard = BackTrack(new BoardModel(tempBoard), groups);
                 if (tempBoard != null)
                 {
                     board = tempBoard;
                     break;
                 }
             }
             return board;
         }
         public static BoardModel BackTrack1(BoardModel board, List<List<List<Tuple<int, int>>>> groups)
         {
             Stack<BoardModel> boardStack = new Stack<BoardModel>();
             boardStack.Push(new BoardModel(board));
             DateTime timeout = DateTime.Now.AddMilliseconds(10000);
             while (boardStack.Count > 0 && DateTime.Now < timeout)
             {
                 BoardModel currentBoard = boardStack.Pop();
                 Tuple<int, int> coords = GetNextCoordinates(currentBoard.currentX, currentBoard.currentY, currentBoard.BoardValues.Length);
                 int nextX = coords.Item1;
                 int nextY = coords.Item2;
                 Cell currentCell = currentBoard.BoardValues[currentBoard.currentX][currentBoard.currentY];
                 if (nextX == -1 || nextY == -1) // End of board
                 {
                     if (currentCell.CellValue == 0)
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
                     //Pruner.PrunePuzzle(currentBoard, groups);
                     PushNextBoards(boardStack, currentBoard, currentCell, nextX, nextY);
                 }
             }
             return board;
         }

         private static void PushNextBoards(Stack<BoardModel> boardStack, BoardModel currentBoard, Cell currentCell, int nextX, int nextY)
         {
             SkipFilled(currentBoard, currentCell, nextX, nextY);
             if (currentCell.CellValue != 0)
             {
                 boardStack.Push(currentBoard);
             }
             else
             {
                 if (AllCellsHavePossibleValues(currentBoard))
                 {
                     foreach (int value in currentCell.GetPossibleValues())
                     {
                         BoardModel nextBoard = new BoardModel(currentBoard);
                         nextBoard.BoardValues[currentBoard.currentX][currentBoard.currentY].CellValue = value;
                         nextBoard.BoardValues[currentBoard.currentX][currentBoard.currentY].RemovePossibleValue(value);
                         boardStack.Push(nextBoard);
                     }
                 }
             }
         }

         private static bool AllCellsHavePossibleValues(BoardModel board)
         {
             for (int i = 0; i < board.BoardValues.Length; i++)
             {
                 for (int j = 0; j < board.BoardValues[i].Length; j++)
                 {
                     Cell currentCell = board.BoardValues[i][j];
                     if (currentCell.CellValue == 0 && currentCell.GetPossibleValues().Count == 0)
                     {
                         return false;
                     }
                 }
             }
             return true;
         }

         private static void SkipFilled(BoardModel currentBoard, Cell currentCell, int nextX, int nextY)
         {
             while (currentCell.CellValue != 0)
             {
                 Tuple<int, int> res = GetNextCoordinates(nextX, nextY, currentBoard.BoardValues.Length);
                 nextX = res.Item1;
                 nextY = res.Item2;
                 currentBoard.currentX = nextX;
                 currentBoard.currentY = nextY;
                 if(nextX == -1 && nextY == -1)
                 {
                     break;
                 }
                 else
                 {
                     currentCell = currentBoard.BoardValues[nextX][nextY];
                 }
             }
         }

         private static void SkipFilled1(BoardModel currentBoard, Cell currentCell, int nextX, int nextY)
         {
             while (currentCell.CellValue != 0)
             {
                 Tuple<int, int> res = GetNextCoordinates(nextX, nextY, currentBoard.BoardValues.Length);
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
