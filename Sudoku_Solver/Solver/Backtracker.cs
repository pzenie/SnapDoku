using Sudoku_Solver.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku_Solver.Solver
{
    internal static class Backtracker
    {
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

        public static Tuple<int,int> SkipFilled(int[][] board, Tuple<int,int> location)
        {
            while (board[location.Item1][location.Item2] != 0)
            {
                location = GetNextCoordinates(location.Item1, location.Item2, board.Length);
                if (location.Item1 == -1 && location.Item2 == -1) break;
            }
            return location;
        }

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
