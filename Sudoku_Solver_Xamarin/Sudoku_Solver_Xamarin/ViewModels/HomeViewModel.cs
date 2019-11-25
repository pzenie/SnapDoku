using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Caliburn.Micro;
using Sudoku_Solver.Data;
using Sudoku_Solver.Initiation;
using Sudoku_Solver.Solver;
using Xamarin.Forms;

namespace Sudoku_Solver_Xamarin.ViewModels
{
    class HomeViewModel : PropertyChangedBase
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

        public HomeViewModel()
        {
            SolvePuzzleCommand = new Command(execute: () =>
            {
                SolvePuzzle();
            });
            Board = new BoardModel();
            BoardInitiation.InitBasicBoard(Board);
            BoardInitiation.InitCommaSeperatedBoard(Board, TestInputs.UNSOLVED_BOARD_HARD);
        }

        public ICommand SolvePuzzleCommand { private set; get; }

        public void SolvePuzzle()
        {
            Thread thread = new Thread(() =>
            {
                Board = Solver.PuzzleSolver(Board, GroupGetter.GetStandardGroups(Board));
                //ValidSolution = PuzzleVerifier.VerifyPuzzle(Board) ? MagicStrings.SOLVED : MagicStrings.NOT_SOLVED;
            });
            thread.Start();
        }

        public void VerifyPuzzle()
        {
            bool verify = PuzzleVerifier.VerifyPuzzle(Board);
            //ValidSolution = verify ? MagicStrings.VALID_SOLUTION : MagicStrings.INVALID_SOLUTION;
        }
    }
}
