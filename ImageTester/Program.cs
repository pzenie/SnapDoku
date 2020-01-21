using System;
using System.Collections.Generic;

namespace ImageTester
{
    class Program
    {
        /// <summary>
        /// Run parser using a test image.
        /// NOTE: This will not run correctly unless the correct opencvsharp libraries are installed.
        /// By default the libraries to work with xamarin will be installed. These must be uninstalled and replaced with opencvsharp4 for this to work.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                List<int> result = Puzzle_Image_Recognition.Sudoku_Normal.Parser.Solve(new byte[1]);//TODO replace this with test file
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
