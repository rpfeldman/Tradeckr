using DomainModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Globalization;

namespace GENAP_MAUI.ContentViews.Graphs;

public partial class DoubleComparisonLineChart : ContentView
{
    private static readonly SKColor AxisTextColor = SKColor.Parse("#94A3B8");
    private static readonly SKColor GridLineColor = SKColor.Parse("#263241");

    private const int MinDaysVisible = 7;
    private const double PixelsPerLabel = 80;

    public static readonly BindableProperty Transactions1Property = BindableProperty.Create(
        nameof(Transactions1),
        typeof(IEnumerable<TransactionDto>),
        typeof(DoubleComparisonLineChart),
        Array.Empty<TransactionDto>(),
        propertyChanged: OnDataChanged);

    public IEnumerable<TransactionDto> Transactions1
    {
        get => (IEnumerable<TransactionDto>)GetValue(Transactions1Property);
        set => SetValue(Transactions1Property, value);
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

    public static readonly BindableProperty Transactions2Property = BindableProperty.Create(
        nameof(Transactions2),
        typeof(IEnumerable<TransactionDto>),
        typeof(DoubleComparisonLineChart),
        Array.Empty<TransactionDto>(),
        propertyChanged: OnDataChanged);

    public IEnumerable<TransactionDto> Transactions2
    {
        get => (IEnumerable<TransactionDto>)GetValue(Transactions2Property);
        set => SetValue(Transactions2Property, value);
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

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(DoubleComparisonLineChart),
        string.Empty,
        propertyChanged: OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // --- Empty state ---
    public static readonly BindableProperty HasDataProperty = BindableProperty.Create(
        nameof(HasData), typeof(bool), typeof(DoubleComparisonLineChart), false);

    public bool HasData
    {
        get => (bool)GetValue(HasDataProperty);
        private set => SetValue(HasDataProperty, value);
    }

    public static readonly BindableProperty IsEmptyProperty = BindableProperty.Create(
        nameof(IsEmpty), typeof(bool), typeof(DoubleComparisonLineChart), true);

    public bool IsEmpty
    {
        get => (bool)GetValue(IsEmptyProperty);
        private set => SetValue(IsEmptyProperty, value);
    }

    public ISeries[] LineSeriesCollection { get; private set; }
    public ICartesianAxis[] XAxes { get; private set; }
    public ICartesianAxis[] YAxes { get; }

    public string DeltaText { get; private set; } = "0,00$";
    public Color DeltaColor { get; private set; } = Colors.White;
    public string DeltaArrow { get; private set; } = string.Empty;

    private DateOnly[] _dates = [];
    private double _lastWidth;

    public DoubleComparisonLineChart()
    {
        LineSeriesCollection = [];

        XAxes =
        [
            new Axis { IsVisible = false }
        ];

        YAxes =
        [
            new Axis
            {
                Labeler = value => ChartFormat.CompactCurrency(value),
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

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width <= 0 || Math.Abs(width - _lastWidth) < 1) return;
        _lastWidth = width;

        if (_dates.Length > 0)
        {
            XAxes = [BuildXAxis()];
            OnPropertyChanged(nameof(XAxes));
        }
    }

    private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (DoubleComparisonLineChart)bindable;
        control.UpdateChart();
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (DoubleComparisonLineChart)bindable;
        if (control.TitleLabel is null) return;
        control.TitleLabel.Text = (string)newValue;
        control.TitleLabel.IsVisible = !string.IsNullOrWhiteSpace((string)newValue);
    }

    private void UpdateChart()
    {
        var transactions1 = Transactions1?.ToList() ?? [];
        var transactions2 = Transactions2?.ToList() ?? [];

        var hasAny1 = transactions1.Count > 0;
        var hasAny2 = transactions2.Count > 0;

        if (!hasAny1 && !hasAny2)
        {
            HasData = false;
            IsEmpty = true;

            _dates = [];
            LineSeriesCollection = [];
            XAxes = [new Axis { IsVisible = false }];
            ResetDelta();
            NotifyAll();
            return;
        }

        HasData = true;
        IsEmpty = false;

        var dailyTotal1 = hasAny1 ? BuildDailyTotal(transactions1) : new Dictionary<DateOnly, decimal>();
        var dailyTotal2 = hasAny2 ? BuildDailyTotal(transactions2) : new Dictionary<DateOnly, decimal>();

        var (from, to) = ComputeSharedRange(dailyTotal1, dailyTotal2);
        _dates = BuildDateRange(from, to);

        var values1 = AccumulateOverRange(dailyTotal1, _dates);
        var values2 = AccumulateOverRange(dailyTotal2, _dates);

        var sk1 = ToSkColor(Color1);
        var sk2 = ToSkColor(Color2);

        LineSeriesCollection =
        [
            CreateLineSeries(values1, sk1),
            CreateLineSeries(values2, sk2),
            CreateEndMarker(values1, sk1),
            CreateEndMarker(values2, sk2)
        ];

        XAxes = [BuildXAxis()];

        UpdateDelta(values1, values2);
        NotifyAll();
    }

    private void NotifyAll()
    {
        OnPropertyChanged(nameof(LineSeriesCollection));
        OnPropertyChanged(nameof(XAxes));
        OnPropertyChanged(nameof(DeltaText));
        OnPropertyChanged(nameof(DeltaColor));
        OnPropertyChanged(nameof(DeltaArrow));
    }

    private void ResetDelta()
    {
        DeltaText = "0,00$";
        DeltaColor = Colors.White;
        DeltaArrow = string.Empty;
    }

    private void UpdateDelta(double[] values1, double[] values2)
    {
        if (values1.Length == 0 || values2.Length == 0)
        {
            ResetDelta();
            return;
        }

        var last1 = values1[^1];
        var last2 = values2[^1];
        var diff = last1 - last2;

        DeltaText = $"{Math.Abs(diff).ToString("N2", CultureInfo.InvariantCulture)}$";

        if (diff > 0)
        {
            DeltaArrow = "▲";
            DeltaColor = Color1;
        }
        else if (diff < 0)
        {
            DeltaArrow = "▼";
            DeltaColor = Color2;
        }
        else
        {
            DeltaArrow = "=";
            DeltaColor = Colors.White;
        }
    }

    private static Dictionary<DateOnly, decimal> BuildDailyTotal(IEnumerable<TransactionDto> transactions)
    {
        return transactions
            .GroupBy(t => t.Date)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Value));
    }

    private static (DateOnly From, DateOnly To) ComputeSharedRange(
        Dictionary<DateOnly, decimal> daily1,
        Dictionary<DateOnly, decimal> daily2)
    {
        var allKeys = daily1.Keys.Concat(daily2.Keys).ToList();
        var from = allKeys.Min();
        var to = allKeys.Max();

        if (to.DayNumber - from.DayNumber < MinDaysVisible - 1)
        {
            from = to.AddDays(-(MinDaysVisible - 1));
        }

        return (from, to);
    }

    private static DateOnly[] BuildDateRange(DateOnly from, DateOnly to)
    {
        var result = new List<DateOnly>();
        for (var day = from; day <= to; day = day.AddDays(1))
        {
            result.Add(day);
        }
        return result.ToArray();
    }

    private static double[] AccumulateOverRange(
        Dictionary<DateOnly, decimal> dailyTotal,
        DateOnly[] dates)
    {
        var result = new double[dates.Length];
        decimal running = 0;

        for (var i = 0; i < dates.Length; i++)
        {
            if (dailyTotal.TryGetValue(dates[i], out var dayValue))
            {
                running += dayValue;
            }
            result[i] = (double)running;
        }

        return result;
    }

    private Axis BuildXAxis()
    {
        var maxLabels = Math.Max(3, (int)(_lastWidth / PixelsPerLabel));
        var step = Math.Max(1, (int)Math.Ceiling((double)_dates.Length / maxLabels));

        return new Axis
        {
            Labeler = value =>
            {
                var index = (int)Math.Round(value);
                if (index < 0 || index >= _dates.Length) return string.Empty;
                return _dates[index].ToString("dd/MM", CultureInfo.InvariantCulture);
            },
            TextSize = 11,
            LabelsPaint = new SolidColorPaint(AxisTextColor),
            MinStep = step,
            ForceStepToMin = true,
            SeparatorsPaint = null
        };
    }

    private LineSeries<double> CreateLineSeries(double[] values, SKColor color)
    {
        return new LineSeries<double>
        {
            Values = values,
            GeometrySize = 0,
            LineSmoothness = 0.35,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 4 },
            Fill = null,
            GeometryFill = null,
            GeometryStroke = null,
            YToolTipLabelFormatter = point =>
            {
                var index = (int)Math.Round(point.Coordinate.SecondaryValue);
                var dateLabel = index >= 0 && index < _dates.Length
                    ? _dates[index].ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                    : string.Empty;
                var valueLabel = point.Coordinate.PrimaryValue.ToString("N2", CultureInfo.InvariantCulture);
                return $"{dateLabel}\n{valueLabel}$";
            }
        };
    }

    private LineSeries<ObservablePoint> CreateEndMarker(double[] values, SKColor color)
    {
        if (values.Length == 0)
        {
            return new LineSeries<ObservablePoint>
            {
                Values = Array.Empty<ObservablePoint>(),
                IsHoverable = false
            };
        }

        var lastIndex = values.Length - 1;
        var lastValue = values[lastIndex];

        return new LineSeries<ObservablePoint>
        {
            Values = [new ObservablePoint(lastIndex, lastValue)],
            GeometrySize = 14,
            Stroke = null,
            Fill = null,
            GeometryFill = new SolidColorPaint(color),
            GeometryStroke = new SolidColorPaint(SKColors.White.WithAlpha(180))
            {
                StrokeThickness = 2
            },
            DataLabelsPaint = new SolidColorPaint(color)
            {
                SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            },
            DataLabelsSize = 13,
            DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
            DataLabelsFormatter = point =>
                ChartFormat.CompactCurrency(point.Coordinate.PrimaryValue),
            IsHoverable = false
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