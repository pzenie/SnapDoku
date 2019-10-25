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
            BoardValues = FastDeepCloner.DeepCloner.Clone(board.BoardValues);
            for(int i =0; i < BoardValues.Count; i++)
            {
                for(int j = 0; j < BoardValues.Count; j++)
                {
                    BoardValues[i][j] = new Cell(board.BoardValues[i][j]);
                }
            }
            currentX = board.currentX;
            currentY = board.currentY;
        }
    }
}
