using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Caliburn.Micro;
using Sudoku_Solver.Data;
using Sudoku_Solver.Initiation;
using Sudoku_Solver.Solver;
using Sudoku_Solver_Xamarin.Resources;
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

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                NotifyOfPropertyChange(nameof(IsLoading));
            }
        }

        private string statusText;
        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                NotifyOfPropertyChange(nameof(StatusText));
            }
        }

        public HomeViewModel()
        {
            SolvePuzzleCommand = new Command(execute: () =>
            {
                SolvePuzzle();
            });
            VerifyPuzzleCommand = new Command(execute: () =>
            {
                VerifyPuzzle();
            });
            ClearPuzzleCommand = new Command(execute: () =>
            {
                ClearPuzzle();
            });
            IsLoading = false;
            StatusText = "";
            Board = new BoardModel();
            BoardInitiation.InitBasicBoard(Board);
            var result = Puzzle_Image_Recognition.Sudoku_Normal.Parser.Solve(@"\sdsf\test.jpeg");
            var it = result.GetEnumerator();
            for(int i = 0; i < Board.BoardValues.Count; i++)
            {
                for(int j = 0; j < Board.BoardValues[i].Count; j++)
                {
                    Board.BoardValues[i][j].CellValue = it.Current.ToString();
                    it.MoveNext();
                }
            }
           // BoardInitiation.InitCommaSeperatedBoard(Board, TestInputs.UNSOLVED_BOARD_EXTREME);
        }

        public ICommand SolvePuzzleCommand { get; }
        public ICommand VerifyPuzzleCommand { get; }
        public ICommand ClearPuzzleCommand { get; }

        public void SolvePuzzle()
        {
            Thread thread = new Thread(() =>
            {
                IsLoading = true;
                StatusText = MagicStrings.SOLVING;
                Board = Solver.PuzzleSolver(Board, GroupGetter.GetStandardGroups(Board));
                StatusText = PuzzleVerifier.VerifyPuzzle(Board) ? MagicStrings.SOLVED : MagicStrings.NOT_SOLVED;
                IsLoading = false;
            });
            thread.Start();
        }

        public void VerifyPuzzle()
        {
            StatusText = MagicStrings.VERIFYING;
            IsLoading = true;
            bool verify = PuzzleVerifier.VerifyPuzzle(Board);
            IsLoading = false;
            StatusText = verify ? MagicStrings.VALID_SOLUTION : MagicStrings.INVALID_SOLUTION;
        }

        public void ClearPuzzle()
        {
            BoardInitiation.ClearBoard(Board);
        }
    }
}
