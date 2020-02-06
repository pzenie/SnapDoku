using Puzzle_Image_Recognition.Sudoku_Normal;
using System;

namespace ImageTester
{
    class Program
    {
        /// <summary>
        /// Run parser using a test image and print results.
        /// NOTE: This will not run correctly unless the correct opencvsharp libraries are installed in puzzle image recognition.
        /// By default the libraries to work with xamarin will be installed. These must be uninstalled and replaced with opencvsharp4.windows.
        /// </summary>
        static void Main()
        {
            try
            {
                SudokuImageParser p = new SudokuImageParser();
                int[,] board = p.Solve(Properties.Resources.test5);
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
