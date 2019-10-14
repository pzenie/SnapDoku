using System;
using System.Collections.Generic;
using Sudoku_Solver.Models;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Threading;
using Force.DeepCloner;

namespace Sudoku_Solver.ViewModels
{
    class MainViewModel : PropertyChangedBase
    {
        private BoardModel boardPrivate;
        public BoardModel Board
        {
            get { return boardPrivate; }
            set
            {
                boardPrivate = value;
                NotifyOfPropertyChange(nameof(Board));
            }
        }

        private string validSolution;
        public string ValidSolution
        {
            get { return validSolution; }
            set
            {
                validSolution = value;
                NotifyOfPropertyChange(nameof(ValidSolution));
            }
        }

        private string testBoard = "5,3,4,6,7,8,9,1,2," +
                                   "6,7,2,1,9,5,3,4,8," +
                                   "1,9,8,3,4,2,5,6,7," +
                                   "8,5,9,7,6,1,4,2,3," +
                                   "4,2,6,8,5,3,7,9,1," +
                                   "7,1,3,9,2,4,8,5,6," +
                                   "9,6,1,5,3,7,2,8,4," +
                                   "2,8,7,4,1,9,6,3,5," +
                                   "3,4,5,2,8,6,1,7,9";

        private string unsolvedBoardEasy = "0,6,3,4,9,0,0,0,1," +
                                           "0,0,0,0,0,0,7,0,9," +
                                           "0,1,9,0,0,0,0,0,0," +
                                           "0,0,1,0,0,2,9,3,0," +
                                           "9,0,0,1,0,7,0,0,2," +
                                           "0,7,8,9,0,0,4,0,0," +
                                           "0,0,0,0,0,0,8,2,0," +
                                           "3,0,6,0,0,0,0,0,0," +
                                           "4,0,0,0,2,9,1,7,0";

        private string unsolvedBoardMedium = "0,4,0,0,0,2,0,0,1," +
                                             "5,3,0,4,6,9,0,0,8," +
                                             "0,9,0,0,0,0,0,5,0," +
                                             "0,0,0,5,9,0,8,2,0," +
                                             "0,0,0,1,0,6,0,0,0," +
                                             "0,5,3,0,8,4,0,0,0," +
                                             "0,2,0,0,0,0,0,8,0," +
                                             "3,0,0,9,4,7,0,6,2," +
                                             "9,0,0,8,0,0,0,1,0";

        private string unsolvedBoardHard = "2,0,0,0,0,0,6,0,0," +
                                           "0,0,1,0,0,0,2,0,0," +
                                           "0,7,0,0,1,2,0,0,0," +
                                           "9,0,0,2,0,1,7,0,0," +
                                           "0,6,0,9,7,8,0,4,0," +
                                           "0,0,3,5,0,4,0,0,9," +
                                           "0,0,0,4,8,0,0,1,0," +
                                           "0,0,4,0,0,0,9,0,0," +
                                           "0,0,6,0,0,0,0,0,3";

        private string unsolvedBoardExtreme = "8,0,0,0,0,0,0,0,0," +
                                              "0,0,3,6,0,0,0,0,0," +
                                              "0,7,0,0,9,0,2,0,0," +
                                              "0,5,0,0,0,7,0,0,0," +
                                              "0,0,0,0,4,5,7,0,0," +
                                              "0,0,0,1,0,0,0,3,0," +
                                              "0,0,1,0,0,0,0,6,8," +
                                              "0,0,8,5,0,0,0,1,0," +
                                              "0,9,0,0,0,0,4,0,0";

        public MainViewModel()
        {
            Board = new BoardModel();
            InitBasicBoard();
            InitCommaSeperatedBoard(unsolvedBoardHard, 9);
        }

        private void InitCommaSeperatedBoard(string board, int size)
        {
            string[] splitBoard = board.Split(',');
            for(int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int index = (i * size) + j;
                    string value = splitBoard[index];
                    if (value != "0")
                    {
                        Board.BoardValues[i][j].CellValue = value;
                    }
                }
            }
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
                    row.Add(new Cell(leftWall, rightWall, topWall, bottomWall, 9));
                }
                Board.BoardValues.Add(row);
            }
        }

        public void SolvePuzzle()
        {
            Thread thread = new Thread(() =>
            {
                ValidSolution = PuzzleSolver() ? "SOLVED" : "NOT SOLVABLE THE FUCK";

            });
            thread.Start();
        }

        public void VerifyPuzzle()
        {
            bool verify = true;
            verify &= VerifyGroups(GetGroups(Board));
            verify &= VerifyGroups(GetVerticals(Board));
            verify &= VerifyGroups(GetHorizontals(Board));
            ValidSolution = verify ? "Valid Solution!" : "Invalid Solution!";
        }

        private bool PuzzleSolver()
        {
            PrunePuzzle(Board);

            Stack<BoardModel> boardStack = new Stack<BoardModel>();
            boardStack.Push(Board.DeepClone());
            while(boardStack.Count > 0)
            {
                BoardModel currentBoard = boardStack.Pop();
                PrunePuzzle(currentBoard);
                int x = currentBoard.currentX;
                int y = currentBoard.currentY;
                Cell currentCell = currentBoard.BoardValues[x][y];
                if (y < currentBoard.BoardValues.Count - 1)
                {
                    y++;
                }
                else if(x < currentBoard.BoardValues.Count - 1)
                {
                    x++;
                    y = 0;
                }
                else
                {
                    continue;
                }
                if (currentCell.CellValue.Length == 0)
                {
                    foreach(string value in currentCell.PossibleValues)
                    {
                        BoardModel nextBoard = Board.DeepClone();
                        nextBoard.currentX = x;
                        nextBoard.currentY = y;
                        nextBoard.BoardValues[currentBoard.currentX][currentBoard.currentY].CellValue = value;
                        boardStack.Push(nextBoard);
                    }
                }
                else
                {
                    currentBoard.currentX = x;
                    currentBoard.currentY = y;
                    boardStack.Push(currentBoard);
                }
                Board = currentBoard;
            }
            /*Stack<Tuple<Tuple<int, int>, string>> stack = new Stack<Tuple<Tuple<int, int>, string>>();
            for(int i =0; i < Board.Count; i++)
            {
                for(int j = 0; j < Board[i].Count; j++)
                {
                    Cell cell = Board[i][j];
                    if(string.IsNullOrWhiteSpace(cell.CellValue))
                    {
                        string value = "";
                        if(cell.PossibleValues.Count == 0)
                        {
                            if (stack.Count == 0)
                            {
                                return false;
                            }
                            Tuple<Tuple<int, int>, string> next = stack.Pop();
                            i = next.Item1.Item1;
                            j = next.Item1.Item2;
                            value = next.Item2;
                            cell = Board[i][j];
                            cell.CellValue = "";
                            PruneAllCells();
                        }
                        stack.Push(new Tuple<int, int>(i, j));
                        cell.CellValue = value;
                        PruneAllCells();
                    }
                }
            }*/
            return true;
        }

        private void PrunePuzzle(BoardModel board)
        {
            bool changed = true;
            List<List<Cell>> verticals = GetVerticals(board);
            List<List<Cell>> horizontals = GetHorizontals(board);
            List<List<Cell>> groups = GetGroups(board);
            PruneAllCells(board);
            while (changed)
            {
                AssignAndPrune(verticals, board);
                changed = AssignAndPrune(horizontals, board);
                changed |= AssignAndPrune(groups, board);
            }
        }

        private bool AssignAndPrune(List<List<Cell>> group, BoardModel board)
        {
            if(AssignForcedCells(group))
            {
                PruneAllCells(board);
                return true;
            }
            return false;
        }

        /*private void ReInitAllCells()
        {
            foreach(ObservableCollection<Cell> row in Board.BoardValues)
            {
                foreach(Cell cell in row)
                {
                    cell.SetPossibleValues();
                }
            }
        }*/

        private void PruneAllCells(BoardModel board)
        {
            List<List<Cell>> verticals = GetVerticals(board);
            List<List<Cell>> horizontals = GetHorizontals(board);
            List<List<Cell>> groups = GetGroups(board);
            foreach (ObservableCollection<Cell> row in board.BoardValues)
            {
                foreach (Cell cell in row)
                {
                    if (string.IsNullOrWhiteSpace(cell.CellValue))
                    {
                        PruneCell(verticals, cell);
                        PruneCell(horizontals, cell);
                        PruneCell(groups, cell);
                    }
                }
            }
        }

        private void PruneCell(List<List<Cell>> group, Cell cell)
        {
            foreach(List<Cell> grouping in group)
            {
                if(grouping.Contains(cell))
                {
                    foreach(Cell tempCell in grouping)
                    {
                        if (!string.IsNullOrWhiteSpace(tempCell.CellValue))
                        {
                            cell.PossibleValues.Remove(tempCell.CellValue);
                        }
                    }
                    if (cell.PossibleValues.Count == 1)
                    {
                        cell.CellValue = cell.PossibleValues[0];
                    }
                    break;
                }
            }
        }

        private bool AssignForcedCells(List<List<Cell>> group)
        {
            bool changed = false;
            foreach (List<Cell> grouping in group)
            {
                foreach (Cell cell in grouping)
                {
                    if (string.IsNullOrWhiteSpace(cell.CellValue))
                    {
                        foreach (string value in cell.PossibleValues)
                        {
                            bool unique = true;
                            foreach (Cell innerCell in grouping)
                            {
                                if (innerCell != cell && (innerCell.CellValue == value || (string.IsNullOrWhiteSpace(innerCell.CellValue) && innerCell.PossibleValues.Contains(value))))
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

        private bool VerifyGroups(List<List<Cell>> groups)
        {
            foreach (List<Cell> cells in groups)
            {
                HashSet<int> group = new HashSet<int>();
                foreach (Cell cell in cells)
                {
                    if (int.TryParse(cell.CellValue, out int value))
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

        private List<List<Cell>> GetVerticals(BoardModel board)
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

        private List<List<Cell>> GetHorizontals(BoardModel board)
        {
            List<List<Cell>> horizontals = new List<List<Cell>>();
            for(int i =0; i < board.BoardValues.Count; i++)
            {
                horizontals.Add(new List<Cell>());
            }
            foreach(Cell cell in FlattenCells(board))
            {
                horizontals[cell.y].Add(cell);
            }
            return horizontals;
        }

        private List<List<Cell>> GetGroups(BoardModel board)
        {
            List<List<Cell>> groups = new List<List<Cell>>();
            foreach(Cell cell in FlattenCells(board))
            {
                if(!cell.Visited)
                {
                    Queue<Cell> neighbors = new Queue<Cell>();
                    List<Cell> group = new List<Cell>();
                    AddNeighbors(cell, neighbors, board);
                    cell.Visited = true;
                    group.Add(cell);
                    while(neighbors.Count > 0)
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

        private void AddNeighbors(Cell cell, Queue<Cell> neighbors, BoardModel board)
        {
            List<Cell> cells = new List<Cell>();
            if (!cell.BottomWall && cell.x < board.BoardValues.Count-1)
            {
                cells.Add(board.BoardValues[cell.x+1][cell.y]);
            }
            if(!cell.TopWall && cell.x > 0)
            {
                cells.Add(board.BoardValues[cell.x-1][cell.y]);
            }
            if(!cell.LeftWall && cell.y > 0)
            {
                cells.Add(board.BoardValues[cell.x][cell.y - 1]);
            }
            if(!cell.RightWall && cell.y < board.BoardValues.Count-1)
            {
                cells.Add(board.BoardValues[cell.x][cell.y + 1]);
            }
            foreach(var c in cells)
            {
                if(!c.Visited)
                {
                    neighbors.Enqueue(c);
                }
            }
        }

        private List<Cell> FlattenCells(BoardModel board)
        {
            List<Cell> cells = new List<Cell>();
            int x = 0;
            foreach(ObservableCollection<Cell> list in board.BoardValues)
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
