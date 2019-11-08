using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Sudoku_Solver.Models
{
   internal static class GroupGetter
   {
      public static List<List<List<Tuple<int,int>>>> GetStandardGroups(BoardModel board)
      {
         List<List<List<Tuple<int, int>>>> groups = new List<List<List<Tuple<int, int>>>>();
         groups.Add(GetVerticals(board));
         groups.Add(GetHorizontals(board));
         groups.Add(GetGroups(board));
         return groups;
      }
      public static List<List<Tuple<int, int>>> GetVerticals(BoardModel board)
      {
         List<List<Tuple<int, int>>> verticals = new List<List<Tuple<int, int>>>();
         for (int i = 0; i < board.BoardValues.Count; i++)
         {
            verticals.Add(new List<Tuple<int, int>>());
         }
         foreach (Cell cell in FlattenCells(board))
         {
            verticals[cell.x].Add(new Tuple<int,int>(cell.x, cell.y));
         }
         return verticals;
      }

      public static List<List<Tuple<int, int>>> GetHorizontals(BoardModel board)
      {
         List<List<Tuple<int, int>>> horizontals = new List<List<Tuple<int, int>>>();
         for (int i = 0; i < board.BoardValues.Count; i++)
         {
            horizontals.Add(new List<Tuple<int, int>>());
         }
         foreach (Cell cell in FlattenCells(board))
         {
            horizontals[cell.y].Add(new Tuple<int, int>(cell.x, cell.y));
         }
         return horizontals;
      }

      public static List<List<Tuple<int, int>>> GetGroups(BoardModel board)
      {
         List<List<Tuple<int, int>>> groups = new List<List<Tuple<int, int>>>();
         ResetVisited(board);
         foreach (Cell cell in FlattenCells(board))
         {
            if (!cell.Visited)
            {
               Queue<Cell> neighbors = new Queue<Cell>();
               List<Tuple<int, int>> group = new List<Tuple<int, int>>();
               AddNeighbors(cell, neighbors, board);
               cell.Visited = true;
               group.Add(new Tuple<int, int>(cell.x, cell.y));
               while (neighbors.Count > 0)
               {
                  Cell neighbor = neighbors.Dequeue();
                  if (!neighbor.Visited)
                  {
                     neighbor.Visited = true;
                     AddNeighbors(neighbor, neighbors, board);
                     group.Add(new Tuple<int, int>(neighbor.x, neighbor.y));
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
