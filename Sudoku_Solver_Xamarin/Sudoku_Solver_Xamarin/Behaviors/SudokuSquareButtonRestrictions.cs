using Sudoku_Solver_Xamarin.Controls;
using System.Linq;
using Xamarin.Forms;

namespace Sudoku_Solver_Xamarin.Behaviors
{
    class SudokuSquareButtonRestrictions : Behavior<SudokuSquareButton>
    {
        private static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {

            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x)); //Make sure all characters are numbers

                isValid &= args.NewTextValue.Length == 1; //Should be changed to be based off size of board

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);
            }
        }
    }
}
