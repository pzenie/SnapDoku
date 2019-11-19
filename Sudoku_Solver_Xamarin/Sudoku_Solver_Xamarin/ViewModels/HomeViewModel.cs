using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku_Solver_Xamarin.ViewModels
{
    public class HomeViewModel : PropertyChangedBase
    {
        private string test;
        public string Test
        {
            get { return test; }
            set
            {
                test = value;
                NotifyOfPropertyChange(nameof(Test));
            }
        }

        public HomeViewModel()
        {
            Test = "hiiii sup";
        }
    }
}
