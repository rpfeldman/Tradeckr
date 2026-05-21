using GENAP_MAUI.ViewModels;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace GENAP_MAUI.ContentViews.Graphs.BalanceDoughnutChart;

public partial class BalanceDoughnutChart : ContentView
{
    public static readonly BindableProperty IncomeProperty = BindableProperty.Create(
        nameof(Income),
        typeof(decimal),
        typeof(BalanceDoughnutChart),
        0m,
        propertyChanged: OnDataChanged);

    public decimal Income
    {
        get => (decimal)GetValue(IncomeProperty);
        set => SetValue(IncomeProperty, value);
    }

    public static readonly BindableProperty ExpensesProperty = BindableProperty.Create(
        nameof(Expenses),
        typeof(decimal),
        typeof(BalanceDoughnutChart),
        0m,
        propertyChanged: OnDataChanged);

    public decimal Expenses
    {
        get => (decimal)GetValue(ExpensesProperty);
        set => SetValue(ExpensesProperty, value);
    }

    public ISeries[] DoughnutChart { get; }

    public BalanceDoughnutChart()
    {
        DoughnutChart = [
            new PieSeries<ObservableValue>
            {
                Name = "Ingresos",
                Values = [new(0)],
                InnerRadius = 80,
                Fill = new SolidColorPaint(SKColor.Parse("#1EFF03")),
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$",
                ToolTipLabelFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$"
            },
            new PieSeries<ObservableValue>
            {
                Name = "Gastos",
                Values = [new(0)],
                InnerRadius = 80,
                Fill = new SolidColorPaint(SKColor.Parse("#FF0303")),
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$",
                ToolTipLabelFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$"
            }
        ];

        InitializeComponent();
    }

    private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BalanceDoughnutChart)bindable;
        control.UpdateChart();
    }

    private void UpdateChart()
    {
        var balance = Income - Expenses;
        BalanceLabel.Text = $"Balance:\n{balance:N2}$";

        ((PieSeries<ObservableValue>)DoughnutChart[0]).Values = [new((double)Income)];
        ((PieSeries<ObservableValue>)DoughnutChart[1]).Values = [new((double)Expenses)];
    }
}