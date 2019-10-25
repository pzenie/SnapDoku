using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Sudoku_Solver.Models
{
   internal static class GroupGetter
   {
      public static List<List<Cell>> GetVerticals(BoardModel board)
      {
         List<List<Cell>> verticals = new List<List<Cell>>();
         for (int i = 0; i < board.BoardValues.Count; i++)
         {
            verticals.Add(new List<Cell>());
         }
         foreach (Cell cell in FlattenCells(board))
         {
            verticals[cell.x].Add(cell);
         }
         return verticals;
      }

      public static List<List<Cell>> GetHorizontals(BoardModel board)
      {
         List<List<Cell>> horizontals = new List<List<Cell>>();
         for (int i = 0; i < board.BoardValues.Count; i++)
         {
            horizontals.Add(new List<Cell>());
         }
         foreach (Cell cell in FlattenCells(board))
         {
            horizontals[cell.y].Add(cell);
         }
         return horizontals;
      }

      public static List<List<Cell>> GetGroups(BoardModel board)
      {
         List<List<Cell>> groups = new List<List<Cell>>();
         ResetVisited(board);
         foreach (Cell cell in FlattenCells(board))
         {
            if (!cell.Visited)
            {
               Queue<Cell> neighbors = new Queue<Cell>();
               List<Cell> group = new List<Cell>();
               AddNeighbors(cell, neighbors, board);
               cell.Visited = true;
               group.Add(cell);
               while (neighbors.Count > 0)
               {
                  Cell neighbor = neighbors.Dequeue();
                  if (!neighbor.Visited)
                  {
                     neighbor.Visited = true;
                     AddNeighbors(neighbor, neighbors, board);
                     group.Add(neighbor);
                  }
               }
               groups.Add(group);
            }
         }
         return groups;
      }

      public static void ResetVisited(BoardModel board)
      {
         foreach(ObservableCollection<Cell> row in board.BoardValues)
         {
            foreach(Cell cell in row)
            {
               cell.Visited = false;
            }
         }
      }

      private static void AddNeighbors(Cell cell, Queue<Cell> neighbors, BoardModel board)
      {
         List<Cell> cells = new List<Cell>();
         if (!cell.BottomWall && cell.x < board.BoardValues.Count - 1)
         {
            cells.Add(board.BoardValues[cell.x + 1][cell.y]);
         }
         if (!cell.TopWall && cell.x > 0)
         {
            cells.Add(board.BoardValues[cell.x - 1][cell.y]);
         }
         if (!cell.LeftWall && cell.y > 0)
         {
            cells.Add(board.BoardValues[cell.x][cell.y - 1]);
         }
         if (!cell.RightWall && cell.y < board.BoardValues.Count - 1)
         {
            cells.Add(board.BoardValues[cell.x][cell.y + 1]);
         }
         foreach (var c in cells)
         {
            if (!c.Visited)
            {
               neighbors.Enqueue(c);
            }
         }
      }

      private static List<Cell> FlattenCells(BoardModel board)
      {
         List<Cell> cells = new List<Cell>();
         int x = 0;
         foreach (ObservableCollection<Cell> list in board.BoardValues)
         {
            int y = 0;
            foreach (Cell c in list)
            {
               c.x = x;
               c.y = y;
               cells.Add(c);
               y++;
            }
            x++;
         }
         return cells;
      }
   }
}
