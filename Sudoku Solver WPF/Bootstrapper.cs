using Caliburn.Micro;
using SnapDoku_WPF.ViewModels;
using System.Windows;

namespace SnapDoku_WPF
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
