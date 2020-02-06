using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Puzzle_Image_Recognition.Sudoku_Normal;
using Sudoku_Solver.Solver;
using Sudoku_Solver_Xamarin.DependencyServiceInterfaces;
using Sudoku_Solver_Shared.Models;
using Sudoku_Solver_Xamarin.Resources;
using Xamarin.Forms;
using Sudoku_Solver_Shared.Initiation;

namespace Sudoku_Solver_Xamarin.ViewModels
{
    class HomeViewModel : PropertyChangedBase
    {
        private readonly SudokuImageParser parser;

        public ObservableCollection<ObservableCollection<ObservableCell>> Board { get; set; }

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

        private bool showSnackbar;
        public bool ShowSnackbar
        {
            get { return showSnackbar; }
            set
            {
                showSnackbar = value;
                NotifyOfPropertyChange(nameof(ShowSnackbar));
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
            TakeImageAndParseCommand = new Command(execute: () =>
            {
                TakeImageAndParse();
            });
            NewPuzzleCommand = new Command(execute: () =>
            {
                NewPuzzle();
            });
            CellSelectedCommand = new Command(execute: (object param) =>
            {
                CellSelected(param);
            });
            DigitClickedCommand = new Command(execute: (object digit) =>
            {
                DigitSelected(digit);
            });
            IsLoading = false;
            StatusText = "";
            Board = new ObservableCollection<ObservableCollection<ObservableCell>>();
            parser = new SudokuImageParser();
            BoardInitiation.InitBasicBoard(Board);
        }

        public ICommand SolvePuzzleCommand { get; }
        public ICommand VerifyPuzzleCommand { get; }
        public ICommand ClearPuzzleCommand { get; }
        public ICommand TakeImageAndParseCommand { get; }
        public ICommand NewPuzzleCommand { get; }
        public ICommand CellSelectedCommand { get; }
        public ICommand DigitClickedCommand { get; }

        public void SolvePuzzle()
        {
            Thread thread = new Thread(() =>
            {
                IsLoading = true;
                int[][] boardArray = BoardInitiation.CollectionToIntArray(Board);
                boardArray = Solver.PuzzleSolver(boardArray, GroupGetter.GetStandardGroups(Board));
                bool solved = PuzzleVerifier.VerifyPuzzle(boardArray, GroupGetter.GetStandardGroups(Board));
                UpdateStatus(solved ? MagicStrings.SOLVED : MagicStrings.NOT_SOLVED);
                if (solved) BoardInitiation.IntArrayToCollection(boardArray, Board);
                IsLoading = false;
            });
            thread.Start();
        }
        private void UpdateStatus(string message)
        {
            Thread messageThread = new Thread(() =>
            {
                if (ShowSnackbar) ShowSnackbar = false;
                StatusText = message;
                ShowSnackbar = true;
                Thread.Sleep(3000);
                ShowSnackbar = false;
            });
            messageThread.Start();
        }

        public void VerifyPuzzle()
        {
            IsLoading = true;
            bool verify = PuzzleVerifier.VerifyPuzzle(BoardInitiation.CollectionToIntArray(Board), GroupGetter.GetStandardGroups(Board));
            IsLoading = false;
            UpdateStatus(verify ? MagicStrings.VALID_SOLUTION : MagicStrings.INVALID_SOLUTION);
        }

        public void DigitSelected(object digit)
        {
            string d = (string)digit;
            foreach(var row in Board)
            {
                foreach(ObservableCell cell in row)
                {
                    if(cell.Selected)
                    {
                        cell.CellValue = d;
                        break;
                    }
                }
            }
        }

        public void ClearPuzzle()
        {
            BoardInitiation.ClearBoard(Board);
        }

        public void CellSelected(object cell)
        {
            ObservableCell c = (ObservableCell)cell;
            foreach (var row in Board)
            {
                foreach(ObservableCell cell1 in row)
                {
                    if (!cell1.Equals(c)) cell1.Selected = false;
                }
            }
        }

        public enum Level_e
        {
            Nevermind,
            Very_Easy,
            Easy,
            Medium,
            Hard,
            Very_Hard
        }

        public async void NewPuzzle()
        {
            ClearPuzzle();
            string levelChoice = await Application.Current.MainPage.DisplayActionSheet("Select puzzle difficulty", Level_e.Nevermind.ToString(), null, 
                                                                                        new string[] { Level_e.Very_Easy.ToString().Replace('_', ' '), Level_e.Easy.ToString(), 
                                                                                                       Level_e.Medium.ToString(), Level_e.Hard.ToString(), 
                                                                                                       Level_e.Very_Hard.ToString().Replace('_', ' ')});
            int cut = GetCutLevel(levelChoice);
            if (cut != 0)
            {
                Thread generatePuzzle = new Thread(() =>
                {
                    IsLoading = true;
                    Random r = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute + DateTime.Now.Hour);
                    SudokuSharp.Board s = SudokuSharp.Factory.Puzzle(r.Next(), cut, cut, cut);
                    ConvertSudokuSharpBoardToCollection(s);
                    IsLoading = false;
                });
                generatePuzzle.Start();
            }
        }

        private int GetCutLevel(string level)
        {
            switch (level.Replace(' ', '_'))
            {
                case nameof(Level_e.Very_Easy):
                    return 5;
                case nameof(Level_e.Easy):
                    return 10;
                case nameof(Level_e.Medium):
                    return 15;
                case nameof(Level_e.Hard):
                    return 20;
                case nameof(Level_e.Very_Hard):
                    return 25;
                default:
                    return 0;
            }
        }

        private void ConvertSudokuSharpBoardToCollection(SudokuSharp.Board board)
        {
            for(int i = 0; i < Board.Count; i++)
            {
                for(int j = 0; j < Board[i].Count; j++)
                {
                    string value = board.GetCell(new SudokuSharp.Location(i, j)).ToString();
                    if (value != "0") Board[i][j].CellValue = value;
                    else Board[i][j].CellValue = string.Empty;
                }
            }
        }
        public async void TakeImageAndParse()
        {
            //TODO Should replace this with a messaging service to maintain seperation of viewmodel and view
            bool existing = await Application.Current.MainPage.DisplayAlert("Parse Sudoku", "Would you like to take a photo or upload an existing one?", "Upload", "Take");
            Stream photo;
            if(existing)
            {
                photo = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            }
            else
            {
                photo = await TakePhoto();
            }            
            if (photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    photo.CopyTo(memoryStream);
                    byte[] photoBytes = memoryStream.ToArray();
                    ParsePuzzle(photoBytes);
                }
            }
        }
        private async Task<Stream> TakePhoto()
        {
            PermissionStatus status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted)
            {
                var granted = await CrossPermissions.Current.RequestPermissionsAsync(new Permission[] { Permission.Camera });
                status = granted[Permission.Camera];
            }
            if (status == PermissionStatus.Granted)
            {
                var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() { });
                return photo.GetStream();
            }
            return null;
        }
    

        private void ParsePuzzle(byte[] file)
        {
            int[,] board = parser.Solve(file);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int val = board[i, j];
                    if (val != 0)
                    {
                        Board[i][j].CellValue = val.ToString();
                    }
                    else
                    {
                        Board[i][j].CellValue = string.Empty;
                    }
                }
            }
        }
    }
}
