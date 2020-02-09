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
using Android.Graphics;

namespace Sudoku_Solver_Xamarin.ViewModels
{
    class HomeViewModel : PropertyChangedBase
    {
        // Used for parsing sudoku boards from images
        private readonly SudokuImageParser parser;

        /// <summary>
        /// Holds all data for sudoku board
        /// </summary>
        public ObservableCollection<ObservableCollection<ObservableCell>> Board { get; set; }

        /// <summary>
        /// Tied to any loading proccesses i.e. loading spinners
        /// </summary>
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

        /// <summary>
        /// Whether to show the snackbar or not
        /// </summary>
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

        /// <summary>
        /// Text displayed in the snackbar
        /// </summary>
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

        /// <summary>
        /// Init commands, set default values, set board with basic 9x9 values, init image parser and train knn model
        /// </summary>
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

        /// <summary>
        /// Checks if user wants to clear puzzle and then clears if so
        /// </summary>
        private async void ClearPuzzle()
        {
            bool clear = await Application.Current.MainPage.DisplayAlert("Clear Puzzle", "Are you sure you want to clear?", "Clear", "Nevermind");
            if (clear)
            {
                BoardInitiation.ClearBoard(Board);
            }
        }

        /// <summary>
        /// Checks if really want to solve, then creates thread to solve the puzzle and verifies if it was solved correctly or not.
        /// </summary>
        public async void SolvePuzzle()
        {
            bool solve = await Application.Current.MainPage.DisplayAlert("Solve Puzzle", "Are you sure you want to solve the puzzle?", "Solve", "Nevermind");
            if (solve)
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        IsLoading = true;
                        int[][] boardArray = BoardInitiation.CollectionToIntArray(Board);
                        boardArray = Solver.PuzzleSolver(boardArray, GroupGetter.GetStandardGroups(Board));
                        bool solved = PuzzleVerifier.VerifyPuzzle(boardArray, GroupGetter.GetStandardGroups(Board));
                        UpdateStatus(solved ? MagicStrings.SOLVED : MagicStrings.NOT_SOLVED);
                        if (solved) BoardInitiation.IntArrayToCollection(boardArray, Board);
                        IsLoading = false;
                    }
                    catch (Exception)
                    {
                        UpdateStatus(MagicStrings.NOT_SOLVED);
                        IsLoading = false;
                    }
                });
                thread.Start();
            }
        }

        /// <summary>
        /// Updates the status message and displays the snackbar with that message
        /// </summary>
        /// <param name="message">The message to update with</param>
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

        /// <summary>
        /// Verifies the puzzle is correctly solved
        /// </summary>
        public void VerifyPuzzle()
        {
            IsLoading = true;
            bool verify = PuzzleVerifier.VerifyPuzzle(BoardInitiation.CollectionToIntArray(Board), GroupGetter.GetStandardGroups(Board));
            IsLoading = false;
            UpdateStatus(verify ? MagicStrings.VALID_SOLUTION : MagicStrings.INVALID_SOLUTION);
        }

        /// <summary>
        /// Puts the specified digit into the selected cell
        /// </summary>
        /// <param name="digit"></param>
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

        /// <summary>
        /// Sets the selected cell value to true
        /// </summary>
        /// <param name="cell"></param>
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

        /// <summary>
        /// Generates a new puzzle based off the selected difficulty
        /// </summary>
        public async void NewPuzzle()
        {
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
                    BoardInitiation.ClearBoard(Board);
                    Random r = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute + DateTime.Now.Hour);
                    SudokuSharp.Board s = SudokuSharp.Factory.Puzzle(r.Next(), cut, cut, cut);
                    ConvertSudokuSharpBoardToCollection(s);
                    IsLoading = false;
                });
                generatePuzzle.Start();
            }
        }

        /// <summary>
        /// Gets the cut level to use for the puzzle generator based off the difficulty level selected
        /// </summary>
        /// <param name="level">Selected difficulty level</param>
        /// <returns>The cut level to cut the puzzle with</returns>
        private int GetCutLevel(string level)
        {
            return (level.Replace(' ', '_')) switch
            {
                nameof(Level_e.Very_Easy) => 5,
                nameof(Level_e.Easy) => 10,
                nameof(Level_e.Medium) => 15,
                nameof(Level_e.Hard) => 20,
                nameof(Level_e.Very_Hard) => 25,
                _ => 0,
            };
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

        /// <summary>
        /// Allows you to choose to either take or upload a photo and then parses the image for the sudoku board
        /// </summary>
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
                Thread thread = new Thread(() => 
                {
                    IsLoading = true;
                    Bitmap source = BitmapFactory.DecodeStream(photo);
                    if (source.Width > source.Height)
                    {
                        Matrix matrix = new Matrix();
                        matrix.PostRotate(90);
                        source = Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, false);
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        source.Compress(Bitmap.CompressFormat.Png, 0, memoryStream);
                        memoryStream.Position = 0;
                        byte[] photoBytes = memoryStream.ToArray();
                        ParsePuzzle(photoBytes);
                    }
                    IsLoading = false;
                });
                thread.Start();
            }
        }

        /// <summary>
        /// Takes a photo with the camera
        /// </summary>
        /// <returns>The stream of the taken photo</returns>
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
                if (photo != null)
                {
                    return photo.GetStream();
                }
            }
            return null;
        }
    
        /// <summary>
        /// Parses an image of a sudoku board using the solver and updates the board with the parsed values
        /// </summary>
        /// <param name="file">The image file in byte[]</param>
        private void ParsePuzzle(byte[] file)
        {
            try
            {
                if (file.Length > 0)
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
                    UpdateStatus("Sudoku board parsed!");
                }
                else
                {
                    UpdateStatus("Could not find a sudoku board");
                }
            }
            catch (Exception)
            {
                UpdateStatus("Could not find a sudoku board");

            }
        }
    }
}
