using DomainModel;
using GENAP_MAUI.InnerComponents;
using GENAP_MAUI.ViewModels;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Globalization;

namespace GENAP_MAUI.ContentViews.Graphs;

public partial class BalanceDoughnutChart : ContentView
{
    private static readonly SKColor IncomeColor = SKColor.Parse("#16C784");
    private static readonly SKColor ExpenseColor = SKColor.Parse("#EA3943");
    private static readonly SKColor NegativeIncomeColor = SKColor.Parse("#F59E0B"); 

    private const string TradingCategoryName = DefaultCategories.TradingCategoryName;

    private const string DefaultUpTitle = "Ingresos";
    private const string DefaultDownTitle = "Gastos";

    public static readonly BindableProperty TransactionsProperty = BindableProperty.Create(
        nameof(Transactions),
        typeof(IEnumerable<GraphableTransactionDto>),
        typeof(BalanceDoughnutChart),
        Array.Empty<GraphableTransactionDto>(),
        propertyChanged: OnDataChanged);

    public IEnumerable<GraphableTransactionDto> Transactions
    {
        get => (IEnumerable<GraphableTransactionDto>)GetValue(TransactionsProperty);
        set => SetValue(TransactionsProperty, value);
    }

    public static readonly BindableProperty SubtractTradingLossFromIncomeProperty = BindableProperty.Create(
        nameof(SubtractTradingLossFromIncome),
        typeof(bool),
        typeof(BalanceDoughnutChart),
        true,
        propertyChanged: OnDataChanged);

    public bool SubtractTradingLossFromIncome
    {
        get => (bool)GetValue(SubtractTradingLossFromIncomeProperty);
        set => SetValue(SubtractTradingLossFromIncomeProperty, value);
    }

    public static readonly BindableProperty UpTitleProperty = BindableProperty.Create(
        nameof(UpTitle),
        typeof(string),
        typeof(BalanceDoughnutChart),
        DefaultUpTitle,
        propertyChanged: OnTitlesChanged);

    public string UpTitle
    {
        get => (string)GetValue(UpTitleProperty);
        set => SetValue(UpTitleProperty, value);
    }

    public static readonly BindableProperty DownTitleProperty = BindableProperty.Create(
        nameof(DownTitle),
        typeof(string),
        typeof(BalanceDoughnutChart),
        DefaultDownTitle,
        propertyChanged: OnTitlesChanged);

    public string DownTitle
    {
        get => (string)GetValue(DownTitleProperty);
        set => SetValue(DownTitleProperty, value);
    }

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title),
        typeof(string),
        typeof(BalanceDoughnutChart),
        string.Empty,
        propertyChanged: OnTitleChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty HasDataProperty = BindableProperty.Create(
        nameof(HasData), typeof(bool), typeof(BalanceDoughnutChart), false);

    public bool HasData
    {
        get => (bool)GetValue(HasDataProperty);
        private set => SetValue(HasDataProperty, value);
    }

    public static readonly BindableProperty IsEmptyProperty = BindableProperty.Create(
        nameof(IsEmpty), typeof(bool), typeof(BalanceDoughnutChart), true);

    public bool IsEmpty
    {
        get => (bool)GetValue(IsEmptyProperty);
        private set => SetValue(IsEmptyProperty, value);
    }

    public ISeries[] DoughnutChart { get; }

    public BalanceDoughnutChart()
    {
        var labelColor = ResolveLabelColor(IsDarkTheme());

        DoughnutChart = [
            new PieSeries<ObservableValue>
            {
                Name = DefaultUpTitle,
                Values = [new(0)],
                InnerRadius = 80,
                Fill = new SolidColorPaint(IncomeColor),
                DataLabelsPaint = CreateLabelPaint(labelColor),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => ChartFormat.CompactCurrency(point.Coordinate.PrimaryValue),
                ToolTipLabelFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$"
            },
            new PieSeries<ObservableValue>
            {
                Name = DefaultDownTitle,
                Values = [new(0)],
                InnerRadius = 80,
                Fill = new SolidColorPaint(ExpenseColor),
                DataLabelsPaint = CreateLabelPaint(labelColor),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => ChartFormat.CompactCurrency(point.Coordinate.PrimaryValue),
                ToolTipLabelFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$"
            }
        ];

        InitializeComponent();

        Loaded += OnChartLoaded;
        Unloaded += OnChartUnloaded;
    }

    private static void OnDataChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BalanceDoughnutChart)bindable;
        control.UpdateChart();
    }

    private static void OnTitlesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BalanceDoughnutChart)bindable;
        control.ApplySeriesNames();
    }

    private void ApplySeriesNames()
    {
        ((PieSeries<ObservableValue>)DoughnutChart[0]).Name =
            string.IsNullOrWhiteSpace(UpTitle) ? DefaultUpTitle : UpTitle;

        ((PieSeries<ObservableValue>)DoughnutChart[1]).Name =
            string.IsNullOrWhiteSpace(DownTitle) ? DefaultDownTitle : DownTitle;

        
        OnPropertyChanged(nameof(DoughnutChart));
    }

    private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (BalanceDoughnutChart)bindable;
        if (control.TitleLabel is null) return;
        control.TitleLabel.Text = (string)newValue;
        control.TitleLabel.IsVisible = !string.IsNullOrWhiteSpace((string)newValue);
    }

    private void UpdateChart()
    {
        var transactions = Transactions?.ToList() ?? [];

        var empty = transactions.Count == 0;
        HasData = !empty;
        IsEmpty = empty;

        if (empty)
        {
            SetBalanceText(0m);
            SetIncomeSlice(0m);
            SetExpenseSlice(0m);
            return;
        }

        decimal income = 0m;
        decimal expenses = 0m;

        foreach (var t in transactions)
        {
            if (t.SignedValue >= 0)
            {
                income += t.SignedValue;
            }
            else if (SubtractTradingLossFromIncome && t.Category == TradingCategoryName)
            {
                income += t.SignedValue; 
            }
            else
            {
                expenses += Math.Abs(t.SignedValue);
            }
        }

        var balance = income - expenses;
        SetBalanceText(balance);

        SetIncomeSlice(income);
        SetExpenseSlice(expenses);
    }

    private void SetBalanceText(decimal balance)
    {
        var culture = CultureInfo.CurrentCulture;

        if (balance % 1 == 0)
        {
            BalanceIntegerSpan.Text = balance.ToString("N0", culture);
            BalanceDecimalSpan.Text = "$";
            return;
        }

        var formatted = balance.ToString("N2", culture);
        var separatorIndex = formatted.LastIndexOf(culture.NumberFormat.NumberDecimalSeparator, StringComparison.Ordinal);

        BalanceIntegerSpan.Text = formatted[..separatorIndex];
        BalanceDecimalSpan.Text = formatted[separatorIndex..] + "$";
    }

    private void SetIncomeSlice(decimal income)
    {
        var slice = (PieSeries<ObservableValue>)DoughnutChart[0];

        slice.Values = [new((double)Math.Abs(income))];

        if (income < 0)
        {
            slice.Fill = new SolidColorPaint(NegativeIncomeColor);
            slice.DataLabelsFormatter = _ => ChartFormat.CompactCurrency((double)income);
            slice.ToolTipLabelFormatter = _ => $"{income:N2}$";
        }
        else
        {
            slice.Fill = new SolidColorPaint(IncomeColor);
            slice.DataLabelsFormatter = point => ChartFormat.CompactCurrency(point.Coordinate.PrimaryValue);
            slice.ToolTipLabelFormatter = point => $"{point.Coordinate.PrimaryValue:N2}$";
        }
    }

    private void SetExpenseSlice(decimal expenses)
    {
        var slice = (PieSeries<ObservableValue>)DoughnutChart[1];
        slice.Values = [new((double)expenses)];
    }

    private static bool IsDarkTheme() => Preferences.Get(PreferenceKeys.UserThemeKey, true);

    private static SKColor ResolveLabelColor(bool isDark)
    {
        var key = isDark ? "TextPrimaryDark" : "TextPrimaryLight";

        if (Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color)
        {
            return new SKColor(
                (byte)(color.Red * 255),
                (byte)(color.Green * 255),
                (byte)(color.Blue * 255),
                (byte)(color.Alpha * 255));
        }

        return isDark ? SKColors.White : SKColor.Parse("#1F2937");
    }
    private static readonly SKTypeface LabelTypeface = SKTypeface.FromFamilyName(
        null,
        SKFontStyleWeight.SemiBold,
        SKFontStyleWidth.Normal,
        SKFontStyleSlant.Upright);

    private static SolidColorPaint CreateLabelPaint(SKColor color) =>
        new(color) { SKTypeface = LabelTypeface };

    private void OnChartLoaded(object? sender, EventArgs e)
    {
        if (Application.Current is not null)
        {
            Application.Current.RequestedThemeChanged -= OnAppThemeChanged;
            Application.Current.RequestedThemeChanged += OnAppThemeChanged;
        }

        ApplyLabelColor(ResolveLabelColor(IsDarkTheme()));
    }

    private void OnChartUnloaded(object? sender, EventArgs e)
    {
        if (Application.Current is not null)
            Application.Current.RequestedThemeChanged -= OnAppThemeChanged;
    }

    private void OnAppThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ApplyLabelColor(ResolveLabelColor(e.RequestedTheme == AppTheme.Dark));
    }

    private void ApplyLabelColor(SKColor color)
    {
        foreach (var series in DoughnutChart.OfType<PieSeries<ObservableValue>>())
            series.DataLabelsPaint = CreateLabelPaint(color);

        OnPropertyChanged(nameof(DoughnutChart));
    }
}
