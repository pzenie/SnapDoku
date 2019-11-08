using Caliburn.Micro;
using System.Collections.ObjectModel;

namespace Sudoku_Solver.Models
{
    internal class BoardModel : PropertyChangedBase
    {
        public ObservableCollection<ObservableCollection<Cell>> BoardValues { get; set; }

        public int currentX;
        public int currentY;

        public BoardModel()
        {
            BoardValues = new ObservableCollection<ObservableCollection<Cell>>();
            currentX = 0;
            currentY = 0;
        }

        public BoardModel(BoardModel board)
        {
            BoardValues = new ObservableCollection<ObservableCollection<Cell>>();
            for(int i =0; i < board.BoardValues.Count; i++)
            {
                BoardValues.Add(new ObservableCollection<Cell>());
                for(int j = 0; j < board.BoardValues.Count; j++)
                {
                    BoardValues[i].Add(new Cell(board.BoardValues[i][j]));
                }
            }
            currentX = board.currentX;
            currentY = board.currentY;
        }
    }
}
