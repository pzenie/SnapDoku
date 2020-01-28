using Sudoku_Solver_Shared.Models;
using System.Collections.ObjectModel;

namespace Sudoku_Solver_Shared.Initiation
{
    public static class BoardInitiation
    {
        public static void InitCommaSeperatedBoard(ObservableCollection<ObservableCollection<ObservableCell>> board, string inputBoard)
        {
            string[] splitBoard = inputBoard.Split(',');
            for (int i = 0; i < board.Count; i++)
            {
                for (int j = 0; j < board.Count; j++)
                {
                    int index = (i * board.Count) + j;
                    string value = splitBoard[index];
                    if (value != "0")
                    {
                        board[i][j].CellValue = value;
                    }
                }
            }
        }

        public static void InitBasicBoard(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            for (int i = 0; i < 9; i++)
            {
                var row = new ObservableCollection<ObservableCell>();
                bool topWall = i % 3 == 0;
                bool bottomWall = i == 2 || i == 5 || i == 8;
                for (int j = 0; j < 9; j++)
                {
                    bool leftWall = j % 3 == 0;
                    bool rightWall = j == 2 || j == 5 || j == 8;
                    row.Add(new ObservableCell(leftWall, rightWall, topWall, bottomWall));
                }
                board.Add(row);
            }
        }

        public static void ClearBoard(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            for (int i = 0; i < board.Count; i++)
            {
                for(int j = 0; j < board.Count; j++)
                {
                    board[i][j].CellValue = string.Empty;
                }
            }
        }
    }
}
