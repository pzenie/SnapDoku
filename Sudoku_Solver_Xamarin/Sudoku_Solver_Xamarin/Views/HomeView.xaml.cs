using Sudoku_Solver_Xamarin.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sudoku_Solver_Xamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            this.BindingContext = new HomeViewModel();
            InitializeComponent();
        }
    }
}