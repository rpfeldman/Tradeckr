using JetBrains.Annotations;

namespace GENAP_MAUI.ContentViews.Navigation;

public partial class BottomNavigationBar : ContentView
{
    public BottomNavigationBar()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            if (Shell.Current is not null)
                Shell.Current.Navigated += OnShellNavigated;
            UpdateActiveState();
        };

        Unloaded += (_, _) =>
        {
            if (Shell.Current is not null)
                Shell.Current.Navigated -= OnShellNavigated;
        };
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
        => UpdateActiveState();

    private async void OnDefaultTapped(object? s, TappedEventArgs e) => await Go(Routes.DefaultTransaction);
    private async void OnTradeTapped(object? s, TappedEventArgs e) => await Go(Routes.TradingTransactions);
    private async void OnDashboardTapped(object? s, TappedEventArgs e) => await Go(Routes.Dashboard);
    private async void OnCategoriesTapped(object? s, TappedEventArgs e) => await Go(Routes.Categories);
    private async void OnGraphsTapped(object? s, TappedEventArgs e) => await Go(Routes.Graphs);

    private static async Task Go(string route)
    {
        var current = Shell.Current?.CurrentState?.Location?.OriginalString ?? string.Empty;
        if (current.Contains(route)) return;
        await Shell.Current!.GoToAsync($"//{route}");
    }

    private void UpdateActiveState()
    {
        var current = Shell.Current?.CurrentState?.Location?.OriginalString ?? string.Empty;

        SetItem(IconDefault, TextDefault, current.Contains(Routes.DefaultTransaction));
        SetItem(IconTrade, TextTrade, current.Contains(Routes.TradingTransactions));
        SetItem(IconCategories, TextCategories, current.Contains(Routes.Categories));
        SetItem(IconGraphs, TextGraphs, current.Contains(Routes.Graphs));

        FabBorder.Opacity = current.Contains(Routes.Dashboard) ? 1.0 : 0.85;
    }

    private void SetItem(Label icon, Label text, bool active)
    {
        var activeColor = (Color)Application.Current!.Resources[
            Application.Current.RequestedTheme == AppTheme.Dark ? "AccentDark" : "AccentLight"];
        var inactiveColor = (Color)Application.Current.Resources[
            Application.Current.RequestedTheme == AppTheme.Dark ? "TextSecondaryDark" : "TextSecondaryLight"];

        text.TextColor = active ? activeColor : inactiveColor;
        icon.Opacity = active ? 1.0 : 0.6;
    }
}