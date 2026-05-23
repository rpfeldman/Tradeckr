using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace GENAP_MAUI.ContentViews.Graphs;

public partial class DoubleComparisonLineChart : ContentView
{
    private static readonly SKColor AxisTextColor = SKColor.Parse("#94A3B8");
    private static readonly SKColor GridLineColor = SKColor.Parse("#263241");

    public static readonly BindableProperty Values1Property = BindableProperty.Create(
        nameof(Values1),
        typeof(decimal[]),
        typeof(DoubleComparisonLineChart),
        Array.Empty<decimal>(),
        propertyChanged: OnDataChanged);

    public decimal[] Values1
    {
        get => (decimal[])GetValue(Values1Property);
        set => SetValue(Values1Property, value);
    }

    public static readonly BindableProperty Color1Property = BindableProperty.Create(
        nameof(Color1),
        typeof(Color),
        typeof(DoubleComparisonLineChart),
        Colors.White,
        propertyChanged: OnDataChanged);

    public Color Color1
    {
        get => (Color)GetValue(Color1Property);
        set => SetValue(Color1Property, value);
    }

    public static readonly BindableProperty Values2Property = BindableProperty.Create(
        nameof(Values2),
        typeof(decimal[]),
        typeof(DoubleComparisonLineChart),
        Array.Empty<decimal>(),
        propertyChanged: OnDataChanged);

    public decimal[] Values2
    {
        get => (decimal[])GetValue(Values2Property);
        set => SetValue(Values2Property, value);
    }

    public static readonly BindableProperty Color2Property = BindableProperty.Create(
        nameof(Color2),
        typeof(Color),
        typeof(DoubleComparisonLineChart),
        Colors.White,
        propertyChanged: OnDataChanged);

    public Color Color2
    {
        get => (Color)GetValue(Color2Property);
        set => SetValue(Color2Property, value);
    }

    public ISeries[] LineSeriesCollection { get; private set; }
    public ICartesianAxis[] XAxes { get; }
    public ICartesianAxis[] YAxes { get; }

    public DoubleComparisonLineChart()
    {
        LineSeriesCollection = [];

        XAxes =
        [
            new Axis
            {
                IsVisible = false
            }
        ];

        YAxes =
        [
            new Axis
            {
                Labeler = value => $"{value:N0}$",
                TextSize = 11,
                LabelsPaint = new SolidColorPaint(AxisTextColor),
                SeparatorsPaint = new SolidColorPaint(GridLineColor.WithAlpha(90))
                {
                    StrokeThickness = 1
                }
            }
        ];

        InitializeComponent();
    }

    private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (DoubleComparisonLineChart)bindable;
        control.UpdateChart();
    }

    private void UpdateChart()
    {
        var series1 = CreateSeries(ToDoubles(Values1), ToSkColor(Color1));
        var series2 = CreateSeries(ToDoubles(Values2), ToSkColor(Color2));

        LineSeriesCollection = [series1, series2];
        OnPropertyChanged(nameof(LineSeriesCollection));
    }

    private static double[] ToDoubles(decimal[] source)
    {
        if (source is null || source.Length == 0)
        {
            return [0d];
        }

        var result = new double[source.Length];

        for (var i = 0; i < source.Length; i++)
        {
            result[i] = (double)source[i];
        }

        return result;
    }

    private static LineSeries<double> CreateSeries(double[] values, SKColor color)
    {
        return new LineSeries<double>
        {
            Values = values,
            GeometrySize = 0,
            LineSmoothness = 0.35,
            Stroke = new SolidColorPaint(color)
            {
                StrokeThickness = 3
            },
            Fill = new LiveChartsCore.SkiaSharpView.Painting.LinearGradientPaint(
                new[]
                {
                    color.WithAlpha(70),
                    color.WithAlpha(18),
                    color.WithAlpha(0)
                },
                new SKPoint(0.5f, 0),
                new SKPoint(0.5f, 1)),
            GeometryFill = null,
            GeometryStroke = null,
            YToolTipLabelFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$"
        };
    }

    private static SKColor ToSkColor(Color color)
    {
        return new SKColor(
            (byte)(color.Red * 255),
            (byte)(color.Green * 255),
            (byte)(color.Blue * 255),
            (byte)(color.Alpha * 255));
    }
}