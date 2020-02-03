using Caliburn.Micro;
using System.Threading;
using Sudoku_Solver_WPF.Resources;
using Sudoku_Solver.Data;
using Sudoku_Solver.Solver;
using Sudoku_Solver_Shared.Initiation;
using System.Collections.ObjectModel;
using Sudoku_Solver_Shared.Models;

namespace Sudoku_Solver_WPF.ViewModels
{
    internal class MainViewModel : PropertyChangedBase
    {
        public ObservableCollection<ObservableCollection<ObservableCell>> Board { get; set; }


        private string validSolution;
        public string ValidSolution
        {
            get { return validSolution; }
            set
            {
                validSolution = value;
                NotifyOfPropertyChange(nameof(ValidSolution));
            }
        }

        public MainViewModel()
        {
            Board = new ObservableCollection<ObservableCollection<ObservableCell>>();
            BoardInitiation.InitBasicBoard(Board);
            BoardInitiation.InitCommaSeperatedBoard(Board, TestInputs.UNSOLVED_BOARD_EASY);
        }

        public void SolvePuzzle()
        {
            Thread thread = new Thread(() =>
            {
                var groups = GroupGetter.GetStandardGroups(Board);
                int[][] boardArray = BoardInitiation.CollectionToIntArray(Board);
                boardArray = Solver.PuzzleSolver(boardArray, groups);
                ValidSolution = PuzzleVerifier.VerifyPuzzle(boardArray, groups) ? MagicStrings.SOLVED : MagicStrings.NOT_SOLVED;
                if (ValidSolution == MagicStrings.SOLVED) BoardInitiation.IntArrayToCollection(boardArray, Board);
            });
            thread.Start();
        }

        public void VerifyPuzzle()
        {
            var groups = GroupGetter.GetStandardGroups(Board);
            bool verify = PuzzleVerifier.VerifyPuzzle(BoardInitiation.CollectionToIntArray(Board), groups);
            ValidSolution = verify ? MagicStrings.VALID_SOLUTION : MagicStrings.INVALID_SOLUTION;
        }
    }
}
