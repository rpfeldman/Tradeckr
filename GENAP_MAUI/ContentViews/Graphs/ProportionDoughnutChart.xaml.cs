using DomainModel;
using GENAP_MAUI.InnerComponents;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;

namespace GENAP_MAUI.ContentViews.Graphs;

public partial class ProportionDoughnutChart : ContentView
{
	private static readonly SKColor FallbackColor = SKColor.Parse("#94A3B8");

	public static readonly BindableProperty TransactionsProperty = BindableProperty.Create(
		nameof(Transactions),
		typeof(List<TransactionDto>),
		typeof(ProportionDoughnutChart),
		new List<TransactionDto>(),
		propertyChanged: OnDataChanged);

	public List<TransactionDto> Transactions
	{
		get => (List<TransactionDto>)GetValue(TransactionsProperty);
		set => SetValue(TransactionsProperty, value);
	}

	public static readonly BindableProperty CategoriesProperty = BindableProperty.Create(
		nameof(Categories),
		typeof(ObservableCollection<CategoryDto>),
		typeof(ProportionDoughnutChart),
		new ObservableCollection<CategoryDto>(),
		propertyChanged: OnDataChanged);

	public ObservableCollection<CategoryDto> Categories
	{
		get => (ObservableCollection<CategoryDto>)GetValue(CategoriesProperty);
		set => SetValue(CategoriesProperty, value);
	}

	public ISeries[] DoughnutSeries { get; private set; }
	public string CenterText { get; private set; } = string.Empty;

	public ProportionDoughnutChart()
	{
		DoughnutSeries = [];
		InitializeComponent();
	}

	private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var control = (ProportionDoughnutChart)bindable;
		control.UpdateChart();
	}

	private void UpdateChart()
	{
		if (Transactions is null || Transactions.Count == 0)
		{
			DoughnutSeries = [];
			CenterText = string.Empty;
			NotifyAll();
			return;
		}

		var grouped = Transactions
			.GroupBy(t => t.Category)
			.Select(g => new
			{
				Name = g.Key,
				Total = g.Sum(t => t.Value)
			})
			.OrderByDescending(g => g.Total)
			.ToList();

		var totalSum = grouped.Sum(g => g.Total);
		var colorMap = BuildColorMap(Categories);

		var series = new List<ISeries>();
		foreach (var group in grouped)
		{
			var color = colorMap.TryGetValue(group.Name, out var c) ? c : FallbackColor;
			series.Add(CreatePieSlice(group.Name, group.Total, totalSum, color));
		}

		DoughnutSeries = series.ToArray();
		CenterText = $"Total\n{totalSum.ToString("N2", CultureInfo.InvariantCulture)}$";

		NotifyAll();
	}

	private void NotifyAll()
	{
		OnPropertyChanged(nameof(DoughnutSeries));
		OnPropertyChanged(nameof(CenterText));
	}

    private static Dictionary<string, SKColor> BuildColorMap(ObservableCollection<CategoryDto> categories)
    {
        var result = new Dictionary<string, SKColor>(StringComparer.OrdinalIgnoreCase);

        if (categories is null) return result;

        foreach (var cat in categories)
        {
            if (string.IsNullOrWhiteSpace(cat.CategoryName)) continue;
            if (cat.Color is null) continue;
            if (string.IsNullOrWhiteSpace(cat.Color.HexColor)) continue;

            try
            {
                var hex = cat.Color.HexColor;
                result[cat.CategoryName] = SKColor.Parse(hex);
            }
            catch
            {
            }
        }

        return result;
    }

    private static PieSeries<ObservableValue> CreatePieSlice(
		string name,
		decimal value,
		decimal totalSum,
		SKColor color)
	{
		var percentage = totalSum > 0 ? (double)(value / totalSum) * 100 : 0;

		return new PieSeries<ObservableValue>
		{
			Name = name,
			Values = [new((double)value)],
			InnerRadius = 80,
			Fill = new SolidColorPaint(color),
			DataLabelsPaint = new SolidColorPaint(SKColors.White),
			DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
			DataLabelsFormatter = point =>
				$"{percentage.ToString("N1", CultureInfo.InvariantCulture)}%",
			ToolTipLabelFormatter = point =>
				$"{name}\n{point.Coordinate.PrimaryValue.ToString("N2", CultureInfo.InvariantCulture)}$ ({percentage.ToString("N1", CultureInfo.InvariantCulture)}%)"
		};
	}
}