using DataServices;
using DomainModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Input;

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
        public partial decimal MonthExpenses { get; set; }

        [ObservableProperty]
        public partial decimal MonthIncome { get; set; }

        [ObservableProperty]
        public partial List<TransactionDto> MonthTransactions { get; set; }

        [RelayCommand]
        public void ChangeTheme()
        {
            Application.Current?.UserAppTheme = Application.Current.UserAppTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        }
        
        [RelayCommand]
        public async Task Load()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var GetMonthExpensesTask = _dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, true);
            var GetMonthIncomeTask = _dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, false);
            var GetMonthTransactionsTask = _dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, order: DataProjectionService.Order.OrderByDateDescending);

            var Transactions = await Task.WhenAll(GetMonthExpensesTask, GetMonthIncomeTask, GetMonthTransactionsTask);

            if (Transactions[0].Success)
            {
                MonthExpenses = DataProjectionService.GetSummedTransactions(Transactions[0].Result!);
            } else { await Shell.Current.DisplayAlertAsync("Error", Transactions[0].ErrorMessage, "Aceptar"); }

            if (Transactions[1].Success)
            {
                MonthIncome = DataProjectionService.GetSummedTransactions(Transactions[1].Result!);
            } else { await Shell.Current.DisplayAlertAsync("Error", Transactions[1].ErrorMessage, "Aceptar"); }

            if (Transactions[2].Success)
            {
                MonthTransactions = Transactions[2].Result!;
            } else { await Shell.Current.DisplayAlertAsync("Error", Transactions[2].ErrorMessage, "Aceptar"); }
        }
    }
}
