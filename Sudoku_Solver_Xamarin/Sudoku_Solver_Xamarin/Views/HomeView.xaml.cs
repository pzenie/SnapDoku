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

        private void FlexButton_Toggled(object sender, ToggledEventArgs e)
        {

        }

        private void FlexButton_Clicked(object sender, System.EventArgs e)
        {

        }
    }
}