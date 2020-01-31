using System;
using System.Collections.Generic;

namespace Sudoku_Solver.Solver
{
    public static class PuzzleVerifier
    {
        public static bool VerifyPuzzle(int[][] board, List<List<List<Tuple<int, int>>>> groups)
        {
            bool verify = true;
            foreach (var group in groups)
            {
                verify &= VerifyGroups(board, group);
            }
            return verify;
        }

        private static bool VerifyGroups(int[][] board, List<List<Tuple<int, int>>> groups)
        {
            foreach (List<Tuple<int, int>> cellLocations in groups)
            {
                HashSet<int> group = new HashSet<int>();
                foreach (Tuple<int, int> cellLocation in cellLocations)
                {
                    int cell = board[cellLocation.Item1][cellLocation.Item2];
                    if (cell > 0 && cell <= board.Length)
                    {
                        if (group.Contains(cell))
                        {
                            return false;
                        }
                        group.Add(cell);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
