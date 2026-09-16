
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GENAP_MAUI.Pages.MainNavigationBarPages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {

        [RelayCommand]
        public async Task GoBack()
        {
            Log.Information($"Moving back");
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task GoToDashboard()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainDashboardPage)}");
        }

        [RelayCommand]
        public async Task PushNavigate(string Route)
        {
            Log.Information($"Moving to {Route}");
            await Shell.Current.GoToAsync(Route, true);
        }

        [RelayCommand]
        public async Task DirectNavigate(string Route)
        {
            Log.Information($"Moving to {Route}");
            await Shell.Current.GoToAsync($"//{Route}", true);
        }
    }
}
