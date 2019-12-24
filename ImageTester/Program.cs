using System;

namespace ImageTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var result = Puzzle_Image_Recognition.Sudoku_Normal.Parser.Solve(@"\sdsf\test.jpeg");
                foreach (int i in result)
                {
                    Console.Write(i + ",");
                } 
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
