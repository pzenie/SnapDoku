using Caliburn.Micro;
using System;

namespace Sudoku_Solver_Xamarin
{
    public static class NavigationServiceExtensions
    {
        public static async void NavigateToViewModel(this INavigationService navigationService, Type viewModelType)
        {
            await navigationService.NavigateToViewModelAsync(viewModelType);
        }
    }
}
