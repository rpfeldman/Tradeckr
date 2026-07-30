using GENAP_MAUI.InnerComponents;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using System.Globalization;

namespace GENAP_MAUI.ContentViews.Graphs;

public partial class DailyColumnChart : ContentView
{
    private static readonly SKColor PositiveColor = SKColor.Parse("#16C784");
    private static readonly SKColor NegativeColor = SKColor.Parse("#EA3943");
    private static readonly SKColor FallbackColor = SKColor.Parse("#94A3B8");

    private static readonly SKColor AxisTextColor = SKColor.Parse("#94A3B8");
    private static readonly SKColor GridLineColor = SKColor.Parse("#263241");
    private static readonly SKColor ZeroLineColor = SKColor.Parse("#CBD5E1");

    private const int MinDaysVisible = 7;
    private const double PixelsPerLabel = 80;

    private const double NeutralDepthRatio = 0.02;

    // Modo de tooltip por serie
    private enum TooltipMode
    {
        RealValue,  
        NeutralZero, 
        Silent      
    }

    public static readonly BindableProperty TransactionsProperty = BindableProperty.Create(
        nameof(Transactions),
        typeof(IEnumerable<GraphableTransactionDto>),
        typeof(DailyColumnChart),
        Array.Empty<GraphableTransactionDto>(),
        propertyChanged: OnDataChanged);

    public IEnumerable<GraphableTransactionDto> Transactions
    {
        get => (IEnumerable<GraphableTransactionDto>)GetValue(TransactionsProperty);
        set => SetValue(TransactionsProperty, value);
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(DailyColumnChart),
        string.Empty,
        propertyChanged: OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty HasDataProperty = BindableProperty.Create(
        nameof(HasData), typeof(bool), typeof(DailyColumnChart), false);

    public bool HasData
    {
        get => (bool)GetValue(HasDataProperty);
        private set => SetValue(HasDataProperty, value);
    }

    public static readonly BindableProperty IsEmptyProperty = BindableProperty.Create(
        nameof(IsEmpty), typeof(bool), typeof(DailyColumnChart), true);

    public bool IsEmpty
    {
        get => (bool)GetValue(IsEmptyProperty);
        private set => SetValue(IsEmptyProperty, value);
    }

    public ISeries[] ColumnSeriesCollection { get; private set; }
    public ICartesianAxis[] XAxes { get; private set; }
    public ICartesianAxis[] YAxes { get; }
    public RectangularSection[] ZeroSection { get; }

    private DateOnly[] _dates = [];
    private double _lastWidth;

    public DailyColumnChart()
    {
        ColumnSeriesCollection = BuildEmptySeries();

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
        var control = (DailyColumnChart)bindable;
        control.UpdateChart();
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (DailyColumnChart)bindable;
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
            ColumnSeriesCollection = BuildEmptySeries();
            XAxes = [new Axis { IsVisible = false }];
            OnPropertyChanged(nameof(ColumnSeriesCollection));
            OnPropertyChanged(nameof(XAxes));
            return;
        }

        HasData = true;
        IsEmpty = false;

        var (dates, dailyNet) = BuildDailyNet(transactions);
        _dates = dates;

        ColumnSeriesCollection = BuildSeries(dailyNet);
        XAxes = [BuildXAxis()];

        OnPropertyChanged(nameof(ColumnSeriesCollection));
        OnPropertyChanged(nameof(XAxes));
    }

    private static (DateOnly[] Dates, double[] Net) BuildDailyNet(IEnumerable<GraphableTransactionDto> transactions)
    {
        var netByDay = transactions
            .GroupBy(t => t.Date)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(t => t.SignedValue));

        var from = netByDay.Keys.Min();
        var to = netByDay.Keys.Max();

        if (to.DayNumber - from.DayNumber < MinDaysVisible - 1)
        {
            from = to.AddDays(-(MinDaysVisible - 1));
        }

        var dates = new List<DateOnly>();
        var net = new List<double>();

        for (var day = from; day <= to; day = day.AddDays(1))
        {
            dates.Add(day);
            net.Add(netByDay.TryGetValue(day, out var value) ? (double)value : 0d);
        }

        return (dates.ToArray(), net.ToArray());
    }

    private ISeries[] BuildSeries(double[] net)
    {
        var n = net.Length;

        var positives = new double?[n];
        var negatives = new double?[n];
        var neutralUp = new double?[n];
        var neutralDown = new double?[n];

        var peak = 0d;
        foreach (var v in net)
        {
            var abs = Math.Abs(v);
            if (abs > peak) peak = abs;
        }
        var neutralDepth = peak > 0 ? peak * NeutralDepthRatio : 1d;

        for (var i = 0; i < n; i++)
        {
            var v = net[i];
            if (v > 0)
            {
                positives[i] = v;
            }
            else if (v < 0)
            {
                negatives[i] = v;
            }
            else
            {
                neutralUp[i] = neutralDepth;
                neutralDown[i] = -neutralDepth;
            }
        }

        return
        [
            CreateColumnSeries(positives, PositiveColor, TooltipMode.RealValue),
            CreateColumnSeries(negatives, NegativeColor, TooltipMode.RealValue),
            CreateColumnSeries(neutralUp, FallbackColor, TooltipMode.NeutralZero), 
            CreateColumnSeries(neutralDown, FallbackColor, TooltipMode.Silent)     
        ];
    }

    private ISeries[] BuildEmptySeries()
    {
        return
        [
            CreateColumnSeries(Array.Empty<double?>(), PositiveColor, TooltipMode.RealValue),
            CreateColumnSeries(Array.Empty<double?>(), NegativeColor, TooltipMode.RealValue),
            CreateColumnSeries(Array.Empty<double?>(), FallbackColor, TooltipMode.NeutralZero),
            CreateColumnSeries(Array.Empty<double?>(), FallbackColor, TooltipMode.Silent)
        ];
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

    private ColumnSeries<double?> CreateColumnSeries(double?[] values, SKColor color, TooltipMode tooltipMode)
    {
        var series = new ColumnSeries<double?>
        {
            Values = values,
            Fill = new SolidColorPaint(color),
            Stroke = null,
            Rx = 2,
            Ry = 2,
            Padding = 0,
            MaxBarWidth = double.MaxValue,
            IgnoresBarPosition = true
        };

        switch (tooltipMode)
        {
            case TooltipMode.RealValue:
                series.YToolTipLabelFormatter = point =>
                {
                    var index = (int)Math.Round(point.Coordinate.SecondaryValue);
                    var dateLabel = index >= 0 && index < _dates.Length
                        ? _dates[index].ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                        : string.Empty;
                    var valueLabel = point.Coordinate.PrimaryValue.ToString("N2", CultureInfo.InvariantCulture);
                    return $"{dateLabel}\n{valueLabel}$";
                };
                break;

            case TooltipMode.NeutralZero:
                series.YToolTipLabelFormatter = point =>
                {
                    var index = (int)Math.Round(point.Coordinate.SecondaryValue);
                    var dateLabel = index >= 0 && index < _dates.Length
                        ? _dates[index].ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                        : string.Empty;
                    return $"{dateLabel}\n0,00$";
                };
                break;

            case TooltipMode.Silent:
                series.IsHoverable = false;
                break;
        }

        return series;
    }
}