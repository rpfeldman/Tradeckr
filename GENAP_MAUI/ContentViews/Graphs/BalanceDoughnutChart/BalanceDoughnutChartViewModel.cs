using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ContentViews.Graphs.BalanceDoughnutChart
{
    public sealed partial class BalanceDoughnutChartViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial ISeries[] DoughnutChart { get; set; }

        [ObservableProperty]
        public partial string InsideChartDataLabel { get; set; } = "Balance:\n0.00$";
        public BalanceDoughnutChartViewModel()
        {
            DoughnutChart = [
             new PieSeries<ObservableValue> { Name = "Ingresos", Values = [ new(0) ], InnerRadius = 80, Fill = new SolidColorPaint(SKColor.Parse("#1EFF03")), DataLabelsPaint = new SolidColorPaint(SKColors.White), DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle, DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$", ToolTipLabelFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$" },
             new PieSeries<ObservableValue> { Name = "Gastos", Values = [ new(0) ], InnerRadius = 80, Fill = new SolidColorPaint(SKColor.Parse("#FF0303")), DataLabelsPaint = new SolidColorPaint(SKColors.White), DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle, DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$", ToolTipLabelFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$"  }
            ];
        }

        public void FillChart(decimal summedIncome, decimal summedExpense)
        {
            var Balance = summedIncome - summedExpense;
            InsideChartDataLabel = $"Balance:\n{Balance:N2}$";

            ((PieSeries<ObservableValue>)DoughnutChart[0]).Values = [new((double)summedIncome)];
            ((PieSeries<ObservableValue>)DoughnutChart[1]).Values = [new((double)summedExpense)];
        }
    }
}
