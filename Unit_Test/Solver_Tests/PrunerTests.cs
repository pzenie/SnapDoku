using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku_Solver.Solver;
using SnapDoku_Shared.Initiation;
using SnapDoku_Shared.Models;

namespace Unit_Test.Solver_Tests
{
    [TestClass]
    public class PrunerTests
    {
        [TestMethod]
        public void TestAssignUniqueCellsEmptyPuzzle()
        {
            var result = RunAssignUniqueCells(TestInputs.EMPTY);
            string resultString = BoardInitiation.IntArrayPuzzleToCommeSeperatedString(result);
            Assert.AreEqual(TestInputs.EMPTY, result);
        }

        private int[][] RunAssignUniqueCells(string testBoard)
        {
            ObservableCollection<ObservableCollection<ObservableCell>> board = new ObservableCollection<ObservableCollection<ObservableCell>>(); ;
            BoardInitiation.InitBasicBoard(board);
            BoardInitiation.InitCommaSeperatedBoard(board, testBoard);
            var groups = GroupGetter.GetStandardGroups(board);
            var realBoard = BoardInitiation.CollectionToIntArray(board);
            var possibeValues = Pruner.InitPossibleValues(realBoard);
            var result = Pruner.AssignUniqueCells(realBoard, possibeValues, groups);
            return result.Item1;
        }
    }
}
