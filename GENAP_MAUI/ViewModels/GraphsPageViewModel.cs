using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class GraphsPageViewModel(DataProjectionService dataProjectionService) : BaseViewModel
    {
        private DataProjectionService _dataProjectionService = dataProjectionService;

        [ObservableProperty]
        public partial decimal TestIncome { get; set; } = 0m;

        [ObservableProperty]
        public partial decimal TestExpense { get; set; } = 0m;

        [ObservableProperty]
        public partial List<TransactionDto> TestBalanceEvolve { get; set; } = [];

        [ObservableProperty]
        public partial List<TransactionDto> Expenses { get; set; } = [];


        [ObservableProperty]
        public partial List<TransactionDto> Income { get; set; } = [];


        [RelayCommand]
        public async Task FillGraphs()
        {
            var expensesTask = _dataProjectionService.GetGlobalResultAsync(true);
            var incomeTask = _dataProjectionService.GetGlobalResultAsync(false);

            var result = await Task.WhenAll(expensesTask, incomeTask);

            TestExpense = result[0];
            TestIncome = result[1];

            TestBalanceEvolve = await _dataProjectionService.GetAllAsync();

            var getExpensesLineTask = _dataProjectionService.GetAllAsync(true);
			var getIncomeLineTask = _dataProjectionService.GetAllAsync(false);

			var ComparisonValues = await Task.WhenAll(getExpensesLineTask, getIncomeLineTask);

            Expenses = ComparisonValues[0];
            Income = ComparisonValues[1];
        }
    }
}
