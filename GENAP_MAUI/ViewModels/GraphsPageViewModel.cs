using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
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
        public partial decimal[] TestBalanceEvolve { get; set; } = { 1500m, 200000m, 250000m, 237000m, 240000m, 23500m, 23700m, 29000m, 187000m, 190000m, 80000m, -90000m, -11000m, -90000m, -70000m, -40000m, -20000m, -15000m, 1000m, 12000m, -900000m};

        [ObservableProperty]
        public partial decimal[] Expenses { get; set; } = { 10m, 11.5m, 8m, 2m, 0, 0, 19m, 30m, 10m, 6m, 9m, 20m };


        [ObservableProperty]
        public partial decimal[] Income { get; set; } = { 0m, 0m, 0m, 0m, 2500, 0, 19m, 200m, 0m, 0m, 0m, 0m };


        [RelayCommand]
        public async Task FillGraphs()
        {
            var expensesTask = _dataProjectionService.GetGlobalResultAsync(true);
            var incomeTask = _dataProjectionService.GetGlobalResultAsync(false);

            var result = await Task.WhenAll(expensesTask, incomeTask);

            TestExpense = result[0];
            TestIncome = result[1];
        }
    }
}
