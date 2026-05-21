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
    }
}
