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
    public sealed partial class GraphsPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        public partial decimal TestIncome { get; set; } = 15350m;

        [ObservableProperty]
        public partial decimal TestExpense { get; set; } = 10953.90m;

        [ObservableProperty]
        public partial decimal[] TestBalanceEvolve { get; set; } = { 1500m, 200000m, 250000m, 237000m, 240000m, 23500m, 23700m, 29000m, 187000m, 190000m, 80000m, -90000m, -11000m, -90000m, -70000m, -40000m, -20000m, -15000m, 1000m, 12000m, -900000m};

        [ObservableProperty]
        public partial decimal[] Expenses { get; set; } = { 200m, 300m, 50000m, 3000m, 0m, 1000m };


        [ObservableProperty]
        public partial decimal[] Income { get; set; } = { 56000m, 300m, 50000m, 3000m, 300000m, 1000m };
    }
}
