using System.Collections.Generic;

namespace Sudoku_Solver.Data
{
    public class Cell
    {
        private List<int> PossibleValues;

        public int CellValue { get; set; }

        public int x, y;
        public void RemovePossibleValue(int value)
        {
            PossibleValues.Remove(value);
        }
        public List<int> GetPossibleValues()
        {
            return PossibleValues;
        }

        public Cell(int x, int y, int value = 0, int boardSize = 9)
        {
            this.x = x;
            this.y = y;
            CellValue = value;
            SetPossibleValues(boardSize);
        }

        public Cell(Cell cell)
        {
            CellValue = cell.CellValue;
            PossibleValues = new List<int>(cell.PossibleValues);
            x = cell.x;
            y = cell.y;
        }

        private void SetPossibleValues(int boardSize)
        {
            PossibleValues = new List<int>();
            for (int i = 1; i <= boardSize; i++)
            {
                PossibleValues.Add(i);
            }
        }
    }
}