using GENAP_MAUI.Pages.MainNavigationBarPages;
using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.TransactionRelatedPages;

public partial class TransactionPage : ContentPage
{
	public TransactionPage(RegistTransactionPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}