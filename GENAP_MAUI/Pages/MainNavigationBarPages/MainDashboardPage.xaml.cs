using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.MainNavigationBarPages;

public partial class MainDashboardPage : ContentPage
{
	public MainDashboardPage(MainDashboardPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainDashboardPageViewModel vm)
        {
            await vm.LoadCommand.ExecuteAsync(false);
        }
    }
}