using GENAP_MAUI.Pages.MainNavigationBarPages;
using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.TransactionRelatedPages;

public partial class TransactionPage : ContentPage
{
	public TransactionPage(TransactionPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}