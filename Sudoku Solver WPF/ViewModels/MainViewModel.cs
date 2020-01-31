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
                BoardModel bModel = CollectionToBoardModel(Board);
                bModel = Solver.PuzzleSolver(bModel, groups);
                BoardModelToCollection(bModel);
                ValidSolution = PuzzleVerifier.VerifyPuzzle(bModel, groups) ? MagicStrings.SOLVED : MagicStrings.NOT_SOLVED;
            });
            thread.Start();
        }

        public void VerifyPuzzle()
        {
            var groups = GroupGetter.GetStandardGroups(Board);
            bool verify = PuzzleVerifier.VerifyPuzzle(CollectionToBoardModel(Board), groups);
            ValidSolution = verify ? MagicStrings.VALID_SOLUTION : MagicStrings.INVALID_SOLUTION;
        }

        private void BoardModelToCollection(BoardModel board)
        {
            for (int i = 0; i < board.BoardValues.Length; i++)
            {
                for (int j = 0; j < board.BoardValues[i].Length; j++)
                {
                    Board[i][j].CellValue = board.BoardValues[i][j].CellValue;
                }
            }
        }

        private BoardModel CollectionToBoardModel(ObservableCollection<ObservableCollection<ObservableCell>> board)
        {
            int[] columns = new int[board.Count];
            for (int i = 0; i < board.Count; i++)
            {
                columns[i] = board[i].Count;
            }
            BoardModel bModel = new BoardModel(board.Count, columns);
            for (int i = 0; i < board.Count; i++)
            {
                for (int j = 0; j < board[i].Count; j++)
                {
                    bModel.BoardValues[i][j] = new Sudoku_Solver.Data.Cell(i, j, board[i][j].CellValue, board.Count);
                }
            }
            return bModel;
        }
    }
}
