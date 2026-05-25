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
        public partial decimal[] Expenses { get; set; } = { 10m, 21.5m, 29.5m, 31.5m, 31.5m, 31.5m, 50.5m, 80.5m, 90.5m, 96.5m, 105.5m, 125.5m };


        [ObservableProperty]
        public partial decimal[] Income { get; set; } = { 0m, 0m, 0m, 0m, 2500, 2500, 2519m, 2719m, 2719m, 2719m, 2719m, 2719m };


        [RelayCommand]
        public async Task FillGraphs()
        {
            var expensesTask = _dataProjectionService.GetGlobalResultAsync(true);
            var incomeTask = _dataProjectionService.GetGlobalResultAsync(false);

            var result = await Task.WhenAll(expensesTask, incomeTask);

            TestExpense = result[0];
            TestIncome = result[1];

            TestBalanceEvolve = await _dataProjectionService.GetAllAsync();
        }
    }
}
