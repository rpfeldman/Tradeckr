using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace GENAP_MAUI.ContentViews.Graphs;

public partial class BasicLineChart : ContentView
{
    private static readonly SKColor AxisTextColor = SKColor.Parse("#94A3B8");
    private static readonly SKColor GridLineColor = SKColor.Parse("#263241");

    public static readonly BindableProperty ValuesProperty = BindableProperty.Create(
        nameof(Values),
        typeof(decimal[]),
        typeof(BasicLineChart),
        Array.Empty<decimal>(),
        propertyChanged: OnDataChanged);

    public decimal[] Values
    {
        get => (decimal[])GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
        nameof(LineColor),
        typeof(Color),
        typeof(BasicLineChart),
        Colors.White,
        propertyChanged: OnColorChanged);

    public Color LineColor
    {
        get => (Color)GetValue(LineColorProperty);
        set => SetValue(LineColorProperty, value);
    }

    public ISeries[] LineSeriesCollection { get; private set; }
    public ICartesianAxis[] XAxes { get; }
    public ICartesianAxis[] YAxes { get; }

    public BasicLineChart()
    {
        LineSeriesCollection =
        [
            CreateSeries([0d], ToSkColor(Colors.White))
        ];

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
        var control = (BasicLineChart)bindable;
        control.UpdateValues();
    }

    private static void OnColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BasicLineChart)bindable;
        control.UpdateColor();
    }

    private void UpdateValues()
    {
        var color = ToSkColor(LineColor);

        if (Values is null || Values.Length == 0)
        {
            LineSeriesCollection = [CreateSeries([0d], color)];
            OnPropertyChanged(nameof(LineSeriesCollection));
            return;
        }

        var doubles = new double[Values.Length];

        for (var i = 0; i < Values.Length; i++)
        {
            doubles[i] = (double)Values[i];
        }

        LineSeriesCollection = [CreateSeries(doubles, color)];
        OnPropertyChanged(nameof(LineSeriesCollection));
    }

    private void UpdateColor()
    {
        UpdateValues();
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