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

	public static readonly BindableProperty TransactionsProperty = BindableProperty.Create(
		nameof(Transactions),
		typeof(List<TransactionDto>),
		typeof(BalanceLineChart),
		new List<TransactionDto>(),
		propertyChanged: OnDataChanged);

	public List<TransactionDto> Transactions
	{
		get => (List<TransactionDto>)GetValue(TransactionsProperty);
		set => SetValue(TransactionsProperty, value);
	}

	public ISeries[] LineSeriesCollection { get; private set; }
	public ICartesianAxis[] XAxes { get; private set; }
	public ICartesianAxis[] YAxes { get; }
	public RectangularSection[] ZeroSection { get; }

	private DateOnly[] _dates = [];

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
				Labeler = value => $"{value:N0}$",
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

	private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var control = (BalanceLineChart)bindable;
		control.UpdateChart();
	}

	private void UpdateChart()
	{
		if (Transactions is null || Transactions.Count == 0)
		{
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

		var (dates, accumulatedValues) = AccumulateByDay(Transactions);
		_dates = dates;

		var hasFewPoints = accumulatedValues.Length <= 3;

		LineSeriesCollection = BuildColoredSegments(accumulatedValues, hasFewPoints).ToArray();
		XAxes = [BuildXAxis()];

		OnPropertyChanged(nameof(LineSeriesCollection));
		OnPropertyChanged(nameof(XAxes));
	}

	private static (DateOnly[] Dates, double[] Values) AccumulateByDay(List<TransactionDto> transactions)
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
			MinStep = 1,
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