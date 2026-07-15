using DomainModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;

namespace GENAP_MAUI.ContentViews.Graphs;

public partial class BalanceLineChart : ContentView
{
    private static readonly SKColor PositiveColor = SKColor.Parse("#16C784");
    private static readonly SKColor NegativeColor = SKColor.Parse("#EA3943");

    private static readonly SKColor AxisTextColor = SKColor.Parse("#94A3B8");
    private static readonly SKColor GridLineColor = SKColor.Parse("#263241");
    private static readonly SKColor ZeroLineColor = SKColor.Parse("#CBD5E1");

    private const int MinDaysVisible = 7;
    private const double PixelsPerLabel = 80;

    public static readonly BindableProperty TransactionsProperty = BindableProperty.Create(
        nameof(Transactions),
        typeof(IEnumerable<TransactionDto>),
        typeof(BalanceLineChart),
        Array.Empty<TransactionDto>(),
        propertyChanged: OnDataChanged);

    public IEnumerable<TransactionDto> Transactions
    {
        get => (IEnumerable<TransactionDto>)GetValue(TransactionsProperty);
        set => SetValue(TransactionsProperty, value);
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(BalanceLineChart),
        string.Empty,
        propertyChanged: OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // --- Empty state ---
    public static readonly BindableProperty HasDataProperty = BindableProperty.Create(
        nameof(HasData), typeof(bool), typeof(BalanceLineChart), false);

    public bool HasData
    {
        get => (bool)GetValue(HasDataProperty);
        private set => SetValue(HasDataProperty, value);
    }

    public static readonly BindableProperty IsEmptyProperty = BindableProperty.Create(
        nameof(IsEmpty), typeof(bool), typeof(BalanceLineChart), true);

    public bool IsEmpty
    {
        get => (bool)GetValue(IsEmptyProperty);
        private set => SetValue(IsEmptyProperty, value);
    }

    public ISeries[] LineSeriesCollection { get; private set; }
    public ICartesianAxis[] XAxes { get; private set; }
    public ICartesianAxis[] YAxes { get; }
    public RectangularSection[] ZeroSection { get; }

    private DateOnly[] _dates = [];
    private double _lastWidth;

    public BalanceLineChart()
    {
        LineSeriesCollection =
        [
            CreateSegmentSeries([new ObservablePoint(0, 0)], PositiveColor, showGeometry: false)
        ];

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

        ZeroSection =
        [
            new RectangularSection
            {
                Yi = 0,
                Yj = 0,
                Stroke = new SolidColorPaint(ZeroLineColor.WithAlpha(120))
                {
                    StrokeThickness = 1,
                    PathEffect = new DashEffect([6, 6])
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
        var control = (BalanceLineChart)bindable;
        control.UpdateChart();
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BalanceLineChart)bindable;
        if (control.TitleLabel is null) return;
        control.TitleLabel.Text = (string)newValue;
        control.TitleLabel.IsVisible = !string.IsNullOrWhiteSpace((string)newValue);
    }

    private void UpdateChart()
    {
        var transactions = Transactions?.ToList() ?? [];

        if (transactions.Count == 0)
        {
            HasData = false;
            IsEmpty = true;

            _dates = [];
            LineSeriesCollection =
            [
                CreateSegmentSeries([new ObservablePoint(0, 0)], PositiveColor, showGeometry: false)
            ];
            XAxes = [new Axis { IsVisible = false }];
            OnPropertyChanged(nameof(LineSeriesCollection));
            OnPropertyChanged(nameof(XAxes));
            return;
        }

        HasData = true;
        IsEmpty = false;

        var (dates, accumulatedValues) = AccumulateByDay(transactions);
        _dates = dates;

        var hasFewPoints = accumulatedValues.Length <= 3;

        LineSeriesCollection = BuildColoredSegments(accumulatedValues, hasFewPoints).ToArray();
        XAxes = [BuildXAxis()];

        OnPropertyChanged(nameof(LineSeriesCollection));
        OnPropertyChanged(nameof(XAxes));
    }

    private static (DateOnly[] Dates, double[] Values) AccumulateByDay(IEnumerable<TransactionDto> transactions)
    {
        var dailyNet = transactions
            .GroupBy(t => t.Date)
            .OrderBy(g => g.Key)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.Depletion ? -t.Value : t.Value));

        var from = dailyNet.Keys.Min();
        var to = dailyNet.Keys.Max();

        if (to.DayNumber - from.DayNumber < MinDaysVisible - 1)
        {
            from = to.AddDays(-(MinDaysVisible - 1));
        }

        var dates = new List<DateOnly>();
        var values = new List<double>();
        decimal running = 0;

        for (var day = from; day <= to; day = day.AddDays(1))
        {
            if (dailyNet.TryGetValue(day, out var dayValue))
            {
                running += dayValue;
            }
            dates.Add(day);
            values.Add((double)running);
        }

        return (dates.ToArray(), values.ToArray());
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
                return _dates[index].ToString("dd/MM");
            },
            TextSize = 11,
            LabelsPaint = new SolidColorPaint(AxisTextColor),
            MinStep = step,
            ForceStepToMin = true,
            SeparatorsPaint = null
        };
    }

    private List<ISeries> BuildColoredSegments(double[] values, bool showGeometry)
    {
        var result = new List<ISeries>();

        if (values.Length == 1)
        {
            var color = values[0] >= 0 ? PositiveColor : NegativeColor;
            result.Add(CreateSegmentSeries(
                [new ObservablePoint(0, values[0])],
                color,
                showGeometry: true));
            return result;
        }

        var currentPoints = new List<ObservablePoint>
        {
            new(0, values[0])
        };

        var currentColor = values[0] >= 0 ? PositiveColor : NegativeColor;

        for (var i = 1; i < values.Length; i++)
        {
            var previousValue = values[i - 1];
            var currentValue = values[i];

            var previousIsPositive = previousValue >= 0;
            var currentIsPositive = currentValue >= 0;

            if (previousIsPositive == currentIsPositive)
            {
                currentPoints.Add(new ObservablePoint(i, currentValue));
                continue;
            }

            var crossingX = (i - 1) + ((0 - previousValue) / (currentValue - previousValue));
            var zeroPoint = new ObservablePoint(crossingX, 0);

            currentPoints.Add(zeroPoint);
            result.Add(CreateSegmentSeries(currentPoints, currentColor, showGeometry));

            currentColor = currentIsPositive ? PositiveColor : NegativeColor;

            currentPoints =
            [
                zeroPoint,
                new ObservablePoint(i, currentValue)
            ];
        }

        result.Add(CreateSegmentSeries(currentPoints, currentColor, showGeometry));
        return result;
    }

    private LineSeries<ObservablePoint> CreateSegmentSeries(
        IEnumerable<ObservablePoint> values,
        SKColor color,
        bool showGeometry)
    {
        return new LineSeries<ObservablePoint>
        {
            Values = values.ToArray(),
            GeometrySize = showGeometry ? 10 : 0,
            LineSmoothness = 0.35,
            Stroke = new SolidColorPaint(color) { StrokeThickness = 3 },
            Fill = new LiveChartsCore.SkiaSharpView.Painting.LinearGradientPaint(
                new[]
                {
                    color.WithAlpha(70),
                    color.WithAlpha(18),
                    color.WithAlpha(0)
                },
                new SKPoint(0.5f, 0),
                new SKPoint(0.5f, 1)),
            GeometryFill = showGeometry ? new SolidColorPaint(color) : null,
            GeometryStroke = null,
            YToolTipLabelFormatter = point =>
            {
                var index = (int)Math.Round(point.Coordinate.SecondaryValue);
                var dateLabel = index >= 0 && index < _dates.Length
                    ? _dates[index].ToString("dd/MM/yyyy")
                    : string.Empty;
                return $"{dateLabel}\n{point.Coordinate.PrimaryValue:N2}$";
            }
        };
    }
}