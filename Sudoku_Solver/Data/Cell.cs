using System.Collections.Generic;

namespace Sudoku_Solver.Data
{
    public class Cell
    {
        private List<string> PossibleValues;

        public string CellValue { get; set; }
        public int x, y;
        public void RemovePossibleValue(string value)
        {
            PossibleValues.Remove(value);
        }
        public List<string> GetPossibleValues()
        {
            return PossibleValues;
        }

        public Cell(string value = "", int boardSize = 9)
        {
            CellValue = value;
            SetPossibleValues(boardSize);
        }
     
        public Cell(Cell cell)
        {
            CellValue = cell.CellValue;
            PossibleValues = new List<string>(cell.PossibleValues);
            x = cell.x;
            y = cell.y;
        }
        
        private void SetPossibleValues(int boardSize)
        {
            PossibleValues = new List<string>();
            for (int i = 1; i <= boardSize; i++)
            {
                PossibleValues.Add(i.ToString());
            }
        }
    }
}