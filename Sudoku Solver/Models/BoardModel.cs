using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver.Models
{
    class BoardModel : PropertyChangedBase
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
    }
}
