using System.Collections.ObjectModel;

namespace Sudoku_Solver.Data
{
    public class BoardModel
    {
        public Cell[][] BoardValues { get; set; }

        public int currentX;
        public int currentY;

        public BoardModel(int rows, int[] columns)
        {
            BoardValues = new Cell[rows][];
            for(int i =0; i < rows; i++)
            {
                BoardValues[i] = new Cell[columns[i]];
            }
            currentX = 0;
            currentY = 0;
        }

        public BoardModel(BoardModel board)
        {
            BoardValues = new Cell[board.BoardValues.Length][];
            for(int i = 0; i < board.BoardValues.Length; i++)
            {
                BoardValues[i] = new Cell[board.BoardValues[i].Length];
                for(int j = 0; j < board.BoardValues[i].Length; j++)
                {
                    BoardValues[i][j] = new Cell(board.BoardValues[i][j]);
                }
            }
            currentX = board.currentX;
            currentY = board.currentY;
        }
    }
}
