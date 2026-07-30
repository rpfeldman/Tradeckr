using DataServices;
using DomainModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Input;
using GENAP_MAUI.InnerComponents;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class MainDashboardPageViewModel : BaseViewModel
    {
        private DataProjectionService _dataProjectionService;

        public MainDashboardPageViewModel(DataProjectionService dataProjectionService)
        {
            _dataProjectionService = dataProjectionService;
            MonthTransactions = [];
        }

        [ObservableProperty]
        public partial TransactionDto[] MonthTransactions { get; set; }

        [ObservableProperty]
        public partial IEnumerable<GraphableTransactionDto> GraphableTransactions { get; set; } = [];

        public string Month { get { return GlobalResources.Months[DateTime.Today.Month]; } }

        [RelayCommand]
        public void ChangeTheme()
        {
            bool IsDarkTheme = Application.Current?.UserAppTheme == AppTheme.Dark;
            Application.Current?.UserAppTheme = IsDarkTheme ? AppTheme.Light : AppTheme.Dark;

            Preferences.Set(PreferenceKeys.UserThemeKey, !IsDarkTheme);
        }
        
        [RelayCommand]
        public async Task Load()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var getMonthTransactions = await _dataProjectionService.GetAllByMonthAsync(today.Month, today.Year);

            if (!getMonthTransactions.Success)
            {
                await Shell.Current.DisplayAlertAsync("Error", getMonthTransactions.InnerError?.ErrorMessage,"Aceptar");
                return;
            }

            MonthTransactions = [.. getMonthTransactions.Result!];
            GraphableTransactions = MonthTransactions.Select(t => new GraphableTransactionDto(t.Depletion ? (t.Value * -1) : t.Value, t.Category, t.Date));
        }
    }
}
