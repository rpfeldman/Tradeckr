using DomainModel;
using GENAP_MAUI.InnerComponents;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace GENAP_MAUI.ContentViews.Graphs;

public partial class ProportionDoughnutChart : ContentView
{
    private static readonly SKColor FallbackColor = SKColor.Parse("#94A3B8");

    private static readonly SKColor TradingColor = SKColor.Parse(ColorsConst.TradingColor);

    private const string TradingCategoryName = DefaultCategories.TradingCategoryName;

    private const double MinLabelPercentage = 4;

    public static readonly BindableProperty TransactionsProperty = BindableProperty.Create(
        nameof(Transactions),
        typeof(IEnumerable<GraphableTransactionDto>),
        typeof(ProportionDoughnutChart),
        Array.Empty<GraphableTransactionDto>(),
        propertyChanged: OnDataChanged);

    public IEnumerable<GraphableTransactionDto> Transactions
    {
        get => (IEnumerable<GraphableTransactionDto>)GetValue(TransactionsProperty);
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

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(ProportionDoughnutChart),
        string.Empty,
        propertyChanged: OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty HasDataProperty = BindableProperty.Create(
        nameof(HasData), typeof(bool), typeof(ProportionDoughnutChart), false);

    public bool HasData
    {
        get => (bool)GetValue(HasDataProperty);
        private set => SetValue(HasDataProperty, value);
    }

    public static readonly BindableProperty IsEmptyProperty = BindableProperty.Create(
        nameof(IsEmpty), typeof(bool), typeof(ProportionDoughnutChart), true);

    public bool IsEmpty
    {
        get => (bool)GetValue(IsEmptyProperty);
        private set => SetValue(IsEmptyProperty, value);
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

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ProportionDoughnutChart)bindable;
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

            DoughnutSeries = [];
            CenterText = string.Empty;
            NotifyAll();
            return;
        }

        HasData = true;
        IsEmpty = false;

        var grouped = transactions
            .GroupBy(t => t.Category)
            .Select(g => new
            {
                Name = g.Key,
                Total = g.Sum(t => Math.Abs(t.SignedValue))
            })
            .OrderByDescending(g => g.Total)
            .ToList();

        var totalSum = grouped.Sum(g => g.Total);
        var colorMap = BuildColorMap(Categories);

        var series = new List<ISeries>();
        foreach (var group in grouped)
        {
            var color = ResolveColor(group.Name, colorMap);
            series.Add(CreatePieSlice(group.Name, group.Total, totalSum, color));
        }

        DoughnutSeries = series.ToArray();
        CenterText = $"Total\n{totalSum.ToString("N0", CultureInfo.InvariantCulture)}$";

        NotifyAll();
    }

    private static SKColor ResolveColor(string categoryName, Dictionary<string, SKColor> colorMap)
    {
        if (categoryName == TradingCategoryName)
            return TradingColor;

        return colorMap.TryGetValue(categoryName, out var color) ? color : FallbackColor;
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
            if (string.IsNullOrWhiteSpace(cat.Name)) continue;
            if (string.IsNullOrWhiteSpace(cat.HexColor)) continue;

            try
            {
                result[cat.Name] = SKColor.Parse(cat.HexColor);
            }
            catch
            {
            }
        }

        return result;
    }

    private static string StripEmojis(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        var sb = new StringBuilder(text.Length);

        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            var element = (string)enumerator.Current;
            var rune = char.ConvertToUtf32(element, 0);

            var isEmoji =
                (rune >= 0x1F000 && rune <= 0x1FAFF) ||
                (rune >= 0x2600 && rune <= 0x27BF) ||
                (rune >= 0x1F1E6 && rune <= 0x1F1FF) ||
                (rune >= 0xFE00 && rune <= 0xFE0F) ||
                rune == 0x200D ||
                rune == 0x20E3;

            if (!isEmoji)
                sb.Append(element);
        }

        return sb.ToString().Trim();
    }

    private static PieSeries<ObservableValue> CreatePieSlice(
        string name,
        decimal value,
        decimal totalSum,
        SKColor color)
    {
        var percentage = totalSum > 0 ? (double)(value / totalSum) * 100 : 0;
        var showLabel = percentage >= MinLabelPercentage;

        var tooltipName = StripEmojis(name);
        if (string.IsNullOrWhiteSpace(tooltipName))
            tooltipName = ""; 

        return new PieSeries<ObservableValue>
        {
            Name = name,
            Values = [new((double)value)],
            InnerRadius = 80,
            Fill = new SolidColorPaint(color),
            DataLabelsPaint = showLabel ? new SolidColorPaint(SKColors.White) : null,
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
            DataLabelsFormatter = point =>
                $"{percentage.ToString("N1", CultureInfo.InvariantCulture)}%",
            ToolTipLabelFormatter = point =>
                $"{tooltipName}\n{point.Coordinate.PrimaryValue.ToString("N2", CultureInfo.InvariantCulture)}$ ({percentage.ToString("N1", CultureInfo.InvariantCulture)}%)"
        };
    }
}