using Caliburn.Micro;
using System.Threading;
using Sudoku_Solver_WPF.Resources;
using Sudoku_Solver.Data;
using Sudoku_Solver.Initiation;
using Sudoku_Solver.Solver;

namespace Sudoku_Solver_WPF.ViewModels
{
    internal class MainViewModel : PropertyChangedBase
    {
        private BoardModel boardPrivate;
        public BoardModel Board
        {
            get { return boardPrivate; }
            set
            {
                boardPrivate = value;
                NotifyOfPropertyChange(nameof(Board));
            }
        }

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
            Board = new BoardModel();
            BoardInitiation.InitBasicBoard(Board);
            BoardInitiation.InitCommaSeperatedBoard(Board, TestInputs.UNSOLVED_BOARD_EXTREME);
        }

        public void SolvePuzzle()
        {
            Thread thread = new Thread(() =>
            {
               Board = Solver.PuzzleSolver(Board, GroupGetter.GetStandardGroups(Board));
               ValidSolution = PuzzleVerifier.VerifyPuzzle(Board) ? MagicStrings.SOLVED : MagicStrings.NOT_SOLVED;

            });
            thread.Start();
        }

        public void VerifyPuzzle()
        {
            bool verify = PuzzleVerifier.VerifyPuzzle(Board);
            ValidSolution = verify ? MagicStrings.VALID_SOLUTION : MagicStrings.INVALID_SOLUTION;
        }
    }
}
