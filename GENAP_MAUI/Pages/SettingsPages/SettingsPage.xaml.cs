using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.SettingsPages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}