using System;
using System.Collections.Generic;

namespace ImageTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                List<int> result = Puzzle_Image_Recognition.Sudoku_Normal.Parser.Solve(@"\sdsf\test.jpeg");
                int row = 0;
                int col = 0;
                int[,] board = new int[9,9];
                foreach (int i in result)
                {
                    if (row >= 9)
                    {
                        row = 0;
                        col++;
                    }
                    board[row,col] = i;
                    row++;
                }
                Console.WriteLine();
                for(int i = 0; i < 9; i++)
                {
                    for(int j = 0; j < 9; j++)
                    {
                        Console.Write(board[i, j] + " ");
                    }
                    Console.WriteLine();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
