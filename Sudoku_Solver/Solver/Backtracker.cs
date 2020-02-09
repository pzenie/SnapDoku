using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku_Solver.Solver
{
    internal static class Backtracker
    {
        /// <summary>
        /// Prunes and then backtracks recursively until solved
        /// </summary>
        /// <param name="board">The sudoku board to solve</param>
        /// <param name="possibleValues">The possible values for every cell in the board</param>
        /// <param name="curLocation">The current location in the board to fill</param>
        /// <param name="groups">The groups each cell belongs to</param>
        /// <returns>the solved board or partially solved board if failure</returns>
        public static int[][] BackTrack(int[][] board, BitArray[][] possibleValues, Tuple<int, int> curLocation,
                                           List<List<List<Tuple<int, int>>>> groups)
        {
            if (curLocation.Item1 == -1 && curLocation.Item2 == -1)
            {
                return board;
            }
            if (NoPossibleValuesForAll(board, possibleValues)) return null;
            bool solutionFound = false;
            for (int i = 0; i < possibleValues[curLocation.Item1][curLocation.Item2].Length; i++)
            {
                if (possibleValues[curLocation.Item1][curLocation.Item2][i])
                {
                    var newBoard = DeepCopyIntArray(board);
                    var newPossibleValues = DeepCopyBitArray(possibleValues);
                    newBoard[curLocation.Item1][curLocation.Item2] = i + 1;
                    newBoard = Pruner.PrunePuzzle(newBoard, groups);
                    Tuple<int, int> nextEmptyCell = SkipFilled(newBoard, new Tuple<int, int>(curLocation.Item1, curLocation.Item2));
                    newPossibleValues = Pruner.PruneAllCells(newBoard, newPossibleValues, groups);
                    newBoard = BackTrack(newBoard, newPossibleValues, nextEmptyCell, groups);
                    if (newBoard != null)
                    {
                        board = newBoard;
                        solutionFound = true;
                        break;
                    }
                }
            }
            if (solutionFound) return board;
            else return null;
        }

        /// <summary>
        /// Checks if there are no possible values for any cells in the board
        /// </summary>
        /// <param name="board">The board to check</param>
        /// <param name="possibleValues">The possible values for every cell in the board</param>
        /// <returns>returns true if there are any cells with no possible values</returns>
        private static bool NoPossibleValuesForAll(int[][] board, BitArray[][] possibleValues)
        {
            for(int i = 0; i < board.Length; i++)
            {
                for(int j = 0; j < board[i].Length; j++)
                {
                    if(board[i][j] == 0)
                    {
                        if (!possibleValues[i][j].Cast<bool>().Contains(true))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Deep copies a 2d int array
        /// </summary>
        /// <param name="board">The 2d int array to copy</param>
        /// <returns>Deep copied 2d int array</returns>
        private static int[][] DeepCopyIntArray(int[][] board)
        {
            int[][] newBoard = new int[board.Length][];
            for(int i = 0; i < board.Length; i++)
            {
                newBoard[i] = new int[board[i].Length];
                for(int j = 0; j < board[i].Length; j++)
                {
                    newBoard[i][j] = board[i][j];
                }
            }
            return newBoard;
        }

        /// <summary>
        /// Deep copies a 2d bitarray array
        /// </summary>
        /// <param name="possibleValues">The 2d Bitarray to deep copy</param>
        /// <returns>Deep copied 2d bitarray</returns>
        private static BitArray[][] DeepCopyBitArray(BitArray[][] possibleValues)
        {
            BitArray[][] newPossibleValues = new BitArray[possibleValues.Length][];
            for(int i = 0; i < possibleValues.Length; i++)
            {
                newPossibleValues[i] = new BitArray[possibleValues[i].Length];
                for(int j = 0; j < possibleValues[i].Length; j++)
                {
                    newPossibleValues[i][j] = new BitArray(possibleValues[i][j].Length, false);
                    for(int k = 0; k < possibleValues[i][j].Length; k++)
                    {
                        newPossibleValues[i][j][k] = possibleValues[i][j][k];
                    }
                }
            }
            return newPossibleValues;
        }

        /// <summary>
        /// Skips past the already filled cells and goes to the next empty cell in the board
        /// </summary>
        /// <param name="board">The board to find the next empty cell</param>
        /// <param name="location">The current location on the  board</param>
        /// <returns>The location of the next empty cell</returns>
        public static Tuple<int,int> SkipFilled(int[][] board, Tuple<int,int> location)
        {
            while (board[location.Item1][location.Item2] != 0)
            {
                location = GetNextCoordinates(location.Item1, location.Item2, board.Length);
                if (location.Item1 == -1 && location.Item2 == -1) break;
            }
            return location;
        }

        /// <summary>
        /// Gets the next coordinate in order based off board size and current location
        /// </summary>
        /// <param name="x">Current x coordinate</param>
        /// <param name="y">Current y coordinate</param>
        /// <param name="size">The size of the board</param>
        /// <returns>The next coordinate</returns>
        private static Tuple<int, int> GetNextCoordinates(int x, int y, int size)
        {
            if (y + 1 < size)
            {
                return new Tuple<int, int>(x, y + 1);
            }
            else if (x + 1 < size)
            {
                return new Tuple<int, int>(x + 1, 0);
            }
            else
            {
                return new Tuple<int, int>(-1, -1);
            }
        }
    }
}
