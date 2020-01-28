using Sudoku_Solver_Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sudoku_Solver_Shared.Initiation
{
    public static class GroupGetter
    {
        public static List<List<List<Tuple<int, int>>>> GetStandardGroups(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            List<List<List<Tuple<int, int>>>> groups = new List<List<List<Tuple<int, int>>>>
            {
                GetVerticals(board),
                GetHorizontals(board),
                GetGroups(board)
            };
            return groups;
        }
        public static List<List<Tuple<int, int>>> GetVerticals(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            List<List<Tuple<int, int>>> verticals = new List<List<Tuple<int, int>>>();
            for (int i = 0; i < board.Count; i++)
            {
                verticals.Add(new List<Tuple<int, int>>());
            }
            foreach (ObservableCell cell in FlattenCells(board))
            {
                verticals[cell.x].Add(new Tuple<int, int>(cell.x, cell.y));
            }
            return verticals;
        }

        public static List<List<Tuple<int, int>>> GetHorizontals(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            List<List<Tuple<int, int>>> horizontals = new List<List<Tuple<int, int>>>();
            for (int i = 0; i < board.Count; i++)
            {
                horizontals.Add(new List<Tuple<int, int>>());
            }
            foreach (ObservableCell cell in FlattenCells(board))
            {
                horizontals[cell.y].Add(new Tuple<int, int>(cell.x, cell.y));
            }
            return horizontals;
        }

        public static List<List<Tuple<int, int>>> GetGroups(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            List<List<Tuple<int, int>>> groups = new List<List<Tuple<int, int>>>();
            ResetVisited(board);
            foreach (ObservableCell cell in FlattenCells(board))
            {
                if (!cell.Visited)
                {
                    Queue<ObservableCell> neighbors = new Queue<ObservableCell>();
                    List<Tuple<int, int>> group = new List<Tuple<int, int>>();
                    AddNeighbors(cell, neighbors, board);
                    cell.Visited = true;
                    group.Add(new Tuple<int, int>(cell.x, cell.y));
                    while (neighbors.Count > 0)
                    {
                        ObservableCell neighbor = neighbors.Dequeue();
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

        public static void ResetVisited(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            foreach (var row in board)
            {
                foreach (ObservableCell cell in row)
                {
                    cell.Visited = false;
                }
            }
        }

        private static void AddNeighbors(ObservableCell cell, Queue<ObservableCell> neighbors, ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            List<ObservableCell> cells = new List<ObservableCell>();
            if (!cell.BottomWall && cell.x < board.Count - 1)
            {
                cells.Add(board[cell.x + 1][cell.y]);
            }
            if (!cell.TopWall && cell.x > 0)
            {
                cells.Add(board[cell.x - 1][cell.y]);
            }
            if (!cell.LeftWall && cell.y > 0)
            {
                cells.Add(board[cell.x][cell.y - 1]);
            }
            if (!cell.RightWall && cell.y < board.Count - 1)
            {
                cells.Add(board[cell.x][cell.y + 1]);
            }
            foreach (var c in cells)
            {
                if (!c.Visited)
                {
                    neighbors.Enqueue(c);
                }
            }
        }

        private static List<ObservableCell> FlattenCells(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            List<ObservableCell> cells = new List<ObservableCell>();
            int x = 0;
            foreach (var list in board)
            {
                int y = 0;
                foreach (ObservableCell c in list)
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
