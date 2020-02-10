using SnapDoku_Shared.Models;
using System;
using System.Collections.ObjectModel;

namespace SnapDoku_Shared.Initiation
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

        public static void IntArrayToCollection(int[][] board, ObservableCollection<ObservableCollection<ObservableCell>> Board)
        {
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    int digit = board[i][j];
                    if (digit != 0)
                    {
                        Board[i][j].CellValue = board[i][j].ToString();
                    }
                    else Board[i][j].CellValue = "";
                }
            }
        }

        public static int[][] CollectionToIntArray(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            int[][] boardArray = new int[board.Count][];
            for (int i = 0; i < board.Count; i++)
            {
                boardArray[i] = new int[board[i].Count];
                for (int j = 0; j < board[i].Count; j++)
                {
                    string digit = board[i][j].CellValue;
                    int number;
                    if (digit.Length == 0) number = 0;
                    else number = Convert.ToInt32(digit);
                    boardArray[i][j] = number;
                }
            }
            return boardArray;
        }

        public static string IntArrayPuzzleToCommeSeperatedString(int[][] board)
        {
            string puzzle = string.Empty;
            for(int i = 0; i < board.Length; i++)
            {
                for(int j =0; j < board[i].Length; j++)
                {
                    if (puzzle.Length != 0) puzzle += ",";
                    puzzle += board[i][j].ToString();
                }
            }
            return puzzle;
        }
    }
}
