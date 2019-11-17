using Caliburn.Micro;
using Sudoku_Solver_WPF.ViewModels;
using System.Windows;

namespace Sudoku_Solver_WPF
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
