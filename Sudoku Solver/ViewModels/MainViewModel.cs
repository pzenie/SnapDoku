using System;
using System.Collections.Generic;
using Sudoku_Solver.Models;
using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace Sudoku_Solver.ViewModels
{
    class MainViewModel : PropertyChangedBase
    {
        public ObservableCollection<ObservableCollection<Cell>> Board { get; set; }

        public MainViewModel()
        {
            Board = new ObservableCollection<ObservableCollection<Cell>>();
            InitBasicBoard();
        }

        private void InitBasicBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                var row = new ObservableCollection<Cell>();
                bool topWall = i % 3 == 0;
                bool bottomWall = i == 2 || i == 5 || i == 8;
                for (int j = 0; j < 9; j++)
                {
                    bool leftWall = j % 3 == 0;
                    bool rightWall = j == 2 || j == 5 || j == 8;
                    row.Add(new Cell(leftWall, rightWall, topWall, bottomWall));
                }
                Board.Add(row);
            }
        }

        public void SolvePuzzle()
        {
            bool passed = VerifyPuzzle();
            passed = false;
        }

        public bool VerifyPuzzle()
        {
            bool verify = true;
            verify &= VerifyGroups(GetGroups());
            verify &= VerifyGroups(GetVerticals());
            verify &= VerifyGroups(GetHorizontals());
            return verify;
        }

        private bool VerifyGroups(List<List<Cell>> groups)
        {
            foreach (List<Cell> cells in groups)
            {
                HashSet<int> group = new HashSet<int>();
                foreach (Cell cell in cells)
                {
                    if (int.TryParse(cell.CellValue, out int value))
                    {
                        if (value > 0 && value <= group.Count)
                        {
                            if (group.Contains(value))
                            {
                                return false;
                            }
                            group.Add(value);
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

        private List<List<Cell>> GetVerticals()
        {
            List<List<Cell>> verticals = new List<List<Cell>>();
            for (int i = 0; i < Board.Count; i++)
            {
                verticals.Add(new List<Cell>());
            }
            foreach (Cell cell in FlattenCells())
            {
                verticals[cell.x].Add(cell);
            }
            return verticals;
        }

        private List<List<Cell>> GetHorizontals()
        {
            List<List<Cell>> horizontals = new List<List<Cell>>();
            for(int i =0; i < Board.Count; i++)
            {
                horizontals.Add(new List<Cell>());
            }
            foreach(Cell cell in FlattenCells())
            {
                horizontals[cell.y].Add(cell);
            }
            return horizontals;
        }

        private List<List<Cell>> GetGroups()
        {
            List<List<Cell>> groups = new List<List<Cell>>();
            foreach(Cell cell in FlattenCells())
            {
                if(!cell.Visited)
                {
                    Queue<Cell> neighbors = new Queue<Cell>();
                    List<Cell> group = new List<Cell>();
                    AddNeighbors(cell, neighbors);
                    cell.Visited = true;
                    group.Add(cell);
                    while(neighbors.Count > 0)
                    {
                        Cell neighbor = neighbors.Dequeue();
                        if (!neighbor.Visited)
                        {
                            neighbor.Visited = true;
                            AddNeighbors(neighbor, neighbors);
                            group.Add(neighbor);
                        }
                    }
                    groups.Add(group);
                }
            }
            return groups;
        }

        private void AddNeighbors(Cell cell, Queue<Cell> neighbors)
        {
            List<Cell> cells = new List<Cell>();
            if (!cell.BottomWall && cell.x < Board.Count-1)
            {
                cells.Add(Board[cell.x+1][cell.y]);
            }
            if(!cell.TopWall && cell.x > 0)
            {
                cells.Add(Board[cell.x-1][cell.y]);
            }
            if(!cell.LeftWall && cell.y > 0)
            {
                cells.Add(Board[cell.x][cell.y - 1]);
            }
            if(!cell.RightWall && cell.y < Board.Count-1)
            {
                cells.Add(Board[cell.x][cell.y + 1]);
            }
            foreach(var c in cells)
            {
                if(!c.Visited)
                {
                    neighbors.Enqueue(c);
                }
            }
        }

        private List<Cell> FlattenCells()
        {
            List<Cell> cells = new List<Cell>();
            int x = 0;
            foreach(ObservableCollection<Cell> list in Board)
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
