using System.Collections.Generic;
using Caliburn.Micro;

namespace Sudoku_Solver.Data
{
    public class Cell : PropertyChangedBase
    {
        private const int offThickness = 0;
        private const int onThickness = 3;

        private string cellValue;
        public string CellValue
        {
            get { return cellValue; }
            set
            {
                int n;
                if (value != null && (value.Length == 0 || int.TryParse(value, out n)))
                {
                    if(value.Length == 0)
                    {
                        SetPossibleValues();
                    }
                    cellValue = value;
                    NotifyOfPropertyChange(nameof(CellValue));
                }
            }
        }

        public int x, y;

        public bool LeftWall
        {
            get { return leftThickness == onThickness; }
            set
            {
                leftThickness = value ? onThickness : offThickness;
                NotifyOfPropertyChange(nameof(LeftThickness));
            }
        }
        public bool RightWall
        {
            get { return rightThickness == onThickness; }
            set
            {
                rightThickness = value ? onThickness : offThickness;
                NotifyOfPropertyChange(nameof(RightThickness));
            }
        }
        public bool TopWall
        {
            get { return topThickness == onThickness; }
            set
            {
                topThickness = value ? onThickness : offThickness;
                NotifyOfPropertyChange(nameof(TopThickness));
            }
        }
        public bool BottomWall
        {
            get { return bottomThickness == onThickness; }
            set
            {
                bottomThickness = value ? onThickness : offThickness;
                NotifyOfPropertyChange(nameof(BottomThickness));
            }
        }


        private double leftThickness;
        public double LeftThickness
        {
            get { return leftThickness; }
        }
        private double rightThickness;
        public double RightThickness
        {
            get { return rightThickness; }
        }
        private double topThickness;
        public double TopThickness
        {
            get { return topThickness; }
        }
        private double bottomThickness;
        public double BottomThickness
        {
            get { return bottomThickness; }
        }
        private string possibleValuesString;
        public string PossibleValuesString
        {
            get
            {
               string stringValue = "";
               foreach (string s in PossibleValues)
               {
                  stringValue += s + " ";
               }
               return stringValue;
            }
            set
            {
               possibleValuesString = value;
               NotifyOfPropertyChange(nameof(PossibleValuesString));
            }
        }
        private List<string> PossibleValues;
        public void RemovePossibleValue(string value)
        {
            PossibleValues.Remove(value);
        }
        public List<string> GetPossibleValues()
        {
            NotifyOfPropertyChange(nameof(PossibleValuesString));
            return PossibleValues;
        }
        private bool visited;
        public bool Visited
        {
            get { return visited; }
            set { visited = value; }
        }
        private int _boardSize;

        public Cell(bool leftWall, bool rightWall, bool topWall, bool bottomWall, int boardSize, string value = "")
        {
            CellValue = value;
            LeftWall = leftWall;
            RightWall = rightWall;
            TopWall = topWall;
            BottomWall = bottomWall;
            _boardSize = boardSize;
            SetPossibleValues();
        }
     
        public Cell(Cell cell)
        {
            CellValue = cell.cellValue;
            LeftWall = cell.LeftWall;
            RightWall = cell.RightWall;
            TopWall = cell.TopWall;
            BottomWall = cell.BottomWall;
            _boardSize = cell._boardSize;
            PossibleValues = new List<string>(cell.PossibleValues);
            x = cell.x;
            y = cell.y;
        }
        
        private void SetPossibleValues()
        {
            PossibleValues = new List<string>();
            for (int i = 1; i <= _boardSize; i++)
            {
                PossibleValues.Add(i.ToString());
            }
        }
    }
}
