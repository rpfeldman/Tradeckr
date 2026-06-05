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
        }

        [ObservableProperty]
        public partial decimal MonthExpenses { get; set; }

        [ObservableProperty]
        public partial decimal MonthIncome { get; set; }

        [ObservableProperty]
        public partial List<TransactionDto> MonthTransactions { get; set; }

        [RelayCommand]
        public async Task FillResults()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var GetMonthExpensesTask = _dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, true);
            var GetMonthIncomeTask = _dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, false);
            var GetMonthTransactionsTask = _dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, order: DataProjectionService.Order.OrderByDateDescending);

            var Transactions = await Task.WhenAll(GetMonthExpensesTask, GetMonthIncomeTask, GetMonthTransactionsTask);

            MonthExpenses = DataProjectionService.GetSummedTransactions(Transactions[0]);
            MonthIncome = DataProjectionService.GetSummedTransactions(Transactions[1]);
            MonthTransactions = Transactions[2];
        }
    }
}
