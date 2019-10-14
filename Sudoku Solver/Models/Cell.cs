using System.Collections.Generic;
using Caliburn.Micro;

namespace Sudoku_Solver.Models
{
    internal class Cell : PropertyChangedBase
    {
        private string cellValue;
        public string CellValue
        {
            get { return cellValue; }
            set
            {
                if (value != null && (value.Length == 0 || int.TryParse(value, out int n)))
                {
                    cellValue = value;
                    NotifyOfPropertyChange(nameof(CellValue));
                }
            }
        }

        public int x, y;

        private const int offThickness = 0;
        private const int onThickness = 3;

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

        public List<string> PossibleValues;
        public bool Visited;
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
        
        public void SetPossibleValues()
        {
            PossibleValues = new List<string>();
            for (int i = 1; i <= _boardSize; i++)
            {
                PossibleValues.Add(i.ToString());
            }
        }
    }
}
