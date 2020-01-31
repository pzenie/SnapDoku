using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku_Solver.Solver
{
    internal static class Pruner
    {
        public static BitArray[][] InitPossibleValues(int[][] board)
        {
            BitArray[][] possibleValues = new BitArray[board.Length][];
            for(int i = 0; i < board.Length; i++)
            {
                possibleValues[i] = new BitArray[board[i].Length];
                for(int j = 0; j < board[i].Length; j++)
                {
                    if(board[i][j] == 0) possibleValues[i][j] = new BitArray(board.Length, true);
                    else possibleValues[i][j] = new BitArray(board.Length, false);
                }
            }
            return possibleValues;
        }

        public static int[][] PrunePuzzle(int[][] board, List<List<List<Tuple<int, int>>>> groups)
        {
            bool changed = true;
            BitArray[][] possibleValues = InitPossibleValues(board);
            while (changed)
            {
                possibleValues = PruneAllCells(board, possibleValues, groups);
                var result = AssignForcedCells(board, possibleValues);
                board = result.Item1;
                possibleValues = result.Item2;
                changed = result.Item3;
                result = AssignUniqueCells(board, possibleValues, groups);
                board = result.Item1;
                possibleValues = result.Item2;
                changed |= result.Item3;
            }
            return board;
        }

        private static BitArray[][] PruneAllCells(int[][] board, BitArray[][] possibleValues, List<List<List<Tuple<int, int>>>> groups)
        {
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 0)
                    {
                        foreach (List<List<Tuple<int, int>>> group in groups)
                        {
                            possibleValues[i][j] = PruneCell(board, possibleValues[i][j], group, new Tuple<int,int>(i,j));
                        }
                    }
                }
            }
            return possibleValues;
        }

        private static BitArray PruneCell(int[][] board, BitArray possibleValues, List<List<Tuple<int,int>>> group, Tuple<int,int> location)
        {
            if (possibleValues.Cast<bool>().Contains(true))
            {
                foreach (List<Tuple<int, int>> grouping in group)
                {
                    if (grouping.Contains(location))
                    {
                        foreach (Tuple<int, int> cellLocation in grouping)
                        {
                            int tempCell = board[cellLocation.Item1][cellLocation.Item2];
                            if (tempCell != 0)
                            {
                                possibleValues[tempCell - 1] = false;
                            }
                        }
                        break;
                    }
                }
            }
            return possibleValues;
        }

        private static Tuple<int[][], BitArray[][], bool> AssignUniqueCells(int[][] board, BitArray[][] possibleValues, List<List<List<Tuple<int, int>>>> groups)
        {
            bool changed = false;
            foreach(var grouping in groups)
            {
                foreach(var group in grouping)
                {
                    Dictionary<int, List<Tuple<int, int>>> uniqueTable = new Dictionary<int, List<Tuple<int, int>>>();
                    foreach (var location in group)
                    {
                        int i = location.Item1;
                        int j = location.Item2;
                        if (board[i][j] == 0)
                        {
                            for (int k = 0; k < possibleValues[i][j].Length; k++)
                            {
                                if (possibleValues[i][j][k])
                                {
                                    if (!uniqueTable.ContainsKey(k + 1)) uniqueTable[k + 1] = new List<Tuple<int, int>>();
                                    uniqueTable[k + 1].Add(location);
                                }
                            }
                        }
                    }
                    foreach(int key in uniqueTable.Keys)
                    {
                        if(uniqueTable[key].Count == 1)
                        {
                            var uniqueLocation = uniqueTable[key].First();
                            board[uniqueLocation.Item1][uniqueLocation.Item2] = key;
                            possibleValues[uniqueLocation.Item1][uniqueLocation.Item2] = new BitArray(board.Length, false);
                            changed = true;
                        }
                    }
                }
            }
            return new Tuple<int[][], BitArray[][], bool>(board, possibleValues, changed);
        }

        private static Tuple<int[][], BitArray[][], bool> AssignForcedCells(int[][] board, BitArray[][] possibleValues)
        {
            bool changed = false;
            for(int i = 0; i < board.Length; i++)
            {
                for(int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == 0)
                    {
                        if (GetCardinality(possibleValues[i][j]) == 1)
                        {
                            for(int k =0; k < possibleValues[i][j].Length; k++)
                            {
                                if (possibleValues[i][j][k])
                                {
                                    board[i][j] = k+1;
                                    possibleValues[i][j] = new BitArray(board.Length, false);
                                    changed = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return new Tuple<int[][], BitArray[][], bool>(board, possibleValues, changed);
        }

        public static Int32 GetCardinality(BitArray bitArray)
        {

            Int32[] ints = new Int32[(bitArray.Count >> 5) + 1];

            bitArray.CopyTo(ints, 0);

            Int32 count = 0;

            // fix for not truncated bits in last integer that may have been set to true with SetAll()
            ints[ints.Length - 1] &= ~(-1 << (bitArray.Count % 32));

            for (Int32 i = 0; i < ints.Length; i++)
            {

                Int32 c = ints[i];

                // magic (http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel)
                unchecked
                {
                    c = c - ((c >> 1) & 0x55555555);
                    c = (c & 0x33333333) + ((c >> 2) & 0x33333333);
                    c = ((c + (c >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
                }

                count += c;

            }

            return count;

        }
    }
}
