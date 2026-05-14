using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.MainNavigationBarPages;

public partial class MainDashboardPage : ContentPage
{
	public MainDashboardPage(MainDashboardPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}