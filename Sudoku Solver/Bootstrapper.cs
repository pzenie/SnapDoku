using Caliburn.Micro;
using Sudoku_Solver.ViewModels;
using System.Windows;

namespace Sudoku_Solver
{
    class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();
        }
    }
}
