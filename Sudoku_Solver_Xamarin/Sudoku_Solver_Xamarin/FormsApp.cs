using Caliburn.Micro;
using Sudoku_Solver_Xamarin.ViewModels;
using Sudoku_Solver_Xamarin.Views;
using Xamarin.Forms;

namespace Sudoku_Solver_Xamarin
{
    public class FormsApp : FormsApplication
    {
        private readonly SimpleContainer container;
        public FormsApp(SimpleContainer container)
        { 
            Initialize();

            this.container = container;

            container.PerRequest<HomeViewModel>();

            DisplayRootView<HomeView>();
        }

        protected override void PrepareViewFirst(NavigationPage navigationPage)
        {
            container.Instance<INavigationService>(new NavigationPageAdapter(navigationPage));
        }
    }
}
