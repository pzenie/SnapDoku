using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Caliburn.Micro;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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
                TakeImageAndParse();//ClearPuzzle();
            });
            TakeImageAndParseCommand = new Command(execute: () =>
            {
                TakeImageAndParse();
            });
            IsLoading = false;
            StatusText = "";
            Board = new BoardModel();
            BoardInitiation.InitBasicBoard(Board);
            
            // BoardInitiation.InitCommaSeperatedBoard(Board, TestInputs.UNSOLVED_BOARD_EXTREME);
        }

        public ICommand SolvePuzzleCommand { get; }
        public ICommand VerifyPuzzleCommand { get; }
        public ICommand ClearPuzzleCommand { get; }
        public ICommand TakeImageAndParseCommand { get; }

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

        public async void TakeImageAndParse()
        {
            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if(status != PermissionStatus.Granted)
            {
                var granted = await CrossPermissions.Current.RequestPermissionsAsync(new Permission[] { Permission.Camera });
                status = granted[Permission.Camera];
            }
            if (status == PermissionStatus.Granted)
            {
                var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });
                if (photo != null)
                {

                    using (var memoryStream = new MemoryStream())
                    {
                        photo.GetStream().CopyTo(memoryStream);
                        byte[] photoBytes = memoryStream.ToArray();
                        ParsePuzzle(photoBytes);
                    }
                }
                else
                {
                    string sdasdas = "wat";
                }
            }
        }

        private void ParsePuzzle(byte[] file)
        {
            var result = Puzzle_Image_Recognition.Sudoku_Normal.Parser.Solve(file);
            int row = 0;
            int col = 0;
            int[,] board = new int[9, 9];
            foreach (int i in result)
            {
                if (row >= 9)
                {
                    row = 0;
                    col++;
                }
                board[row, col] = i;
                row++;
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int val = board[i, j];
                    if (val != 0)
                    {
                        Board.BoardValues[i][j].CellValue = val.ToString();
                    }
                }
            }
        }
    }
}
