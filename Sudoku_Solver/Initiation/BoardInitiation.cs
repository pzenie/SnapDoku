using Sudoku_Solver.Data;
using System.Collections.ObjectModel;

namespace Sudoku_Solver.Initiation
{
    public static class BoardInitiation
    {
        public static void InitCommaSeperatedBoard(BoardModel board, string inputBoard)
        {
            string[] splitBoard = inputBoard.Split(',');
            for (int i = 0; i < board.BoardValues.Count; i++)
            {
                for (int j = 0; j < board.BoardValues.Count; j++)
                {
                    int index = (i * board.BoardValues.Count) + j;
                    string value = splitBoard[index];
                    if (value != "0")
                    {
                        board.BoardValues[i][j].CellValue = value;
                    }
                }
            }
        }

        public static void InitBasicBoard(BoardModel board)
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
                board.BoardValues.Add(row);
            }
        }

        public static void ClearBoard(BoardModel board)
        {
            for (int i = 0; i < board.BoardValues.Count; i++)
            {
                for(int j = 0; j < board.BoardValues[i].Count; j++)
                {
                    board.BoardValues[i][j].CellValue = string.Empty;
                }
            }
        }
    }
}
