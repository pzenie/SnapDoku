using Caliburn.Micro;
using System.Collections.Generic;

namespace SnapDoku_Shared.Models
{
    public class ObservableCell : PropertyChangedBase
    {
        private const int OFF_THICKNESS = 0;
        private const int ON_THICKNESS = 3;

        private string cellValue;
        public string CellValue
        {
            get { return cellValue; }
            set
            {
                cellValue = value;
                NotifyOfPropertyChange(nameof(CellValue));
            }
        }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                NotifyOfPropertyChange(nameof(Selected));
            }
        }

        public bool LeftWall
        {
            get { return LeftThickness == ON_THICKNESS; }
            set
            {
                LeftThickness = value ? ON_THICKNESS : OFF_THICKNESS;
                NotifyOfPropertyChange(nameof(LeftThickness));
                NotifyOfPropertyChange(nameof(LeftWall));
            }
        }
        public bool RightWall
        {
            get { return RightThickness == ON_THICKNESS; }
            set
            {
                RightThickness = value ? ON_THICKNESS : OFF_THICKNESS;
                NotifyOfPropertyChange(nameof(RightThickness));
                NotifyOfPropertyChange(nameof(RightWall));
            }
        }
        public bool TopWall
        {
            get { return TopThickness == ON_THICKNESS; }
            set
            {
                TopThickness = value ? ON_THICKNESS : OFF_THICKNESS;
                NotifyOfPropertyChange(nameof(TopThickness));
                NotifyOfPropertyChange(nameof(TopWall));
            }
        }
        public bool BottomWall
        {
            get { return BottomThickness == ON_THICKNESS; }
            set
            {
                BottomThickness = value ? ON_THICKNESS : OFF_THICKNESS;
                NotifyOfPropertyChange(nameof(BottomThickness));
                NotifyOfPropertyChange(nameof(BottomWall));
            }
        }

        public double LeftThickness { get; private set; }

        public double RightThickness { get; private set; }

        public double TopThickness { get; private set; }
        public double BottomThickness { get; private set; }

        public List<string> PossibleValues { get; set; }
        public bool Visited { get; set; }
        public int x, y;

        public ObservableCell(bool leftWall, bool rightWall, bool topWall, bool bottomWall, string value = "")
        {
            CellValue = value;
            LeftWall = leftWall;
            RightWall = rightWall;
            TopWall = topWall;
            BottomWall = bottomWall;
        }
    }
}