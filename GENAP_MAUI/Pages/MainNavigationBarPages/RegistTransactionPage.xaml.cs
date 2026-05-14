using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.MainNavigationBarPages;

public partial class RegistTransactionPage : ContentPage
{
	public RegistTransactionPage(RegistTransactionPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}