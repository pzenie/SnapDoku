using System;
using System.Collections.Generic;

namespace Sudoku_Solver.Solver
{
    public static class PuzzleVerifier
    {
        /// <summary>
        /// Verifiies the puzzle is solved
        /// </summary>
        /// <param name="board">The board to check</param>
        /// <param name="groups">The groups to check with</param>
        /// <returns>True if solved else false</returns>
        public static bool VerifyPuzzle(int[][] board, List<List<List<Tuple<int, int>>>> groups)
        {
            bool verify = true;
            foreach (var group in groups)
            {
                verify &= VerifyGroups(board, group);
            }
            return verify;
        }

        /// <summary>
        /// Verifies each group is valid
        /// </summary>
        /// <param name="board">The board to check</param>
        /// <param name="groups">The groups to check</param>
        /// <returns>True if all groups valid and filled else false</returns>
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
