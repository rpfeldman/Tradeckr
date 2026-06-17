using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.MainNavigationBarPages;

public partial class TransactionCategoriesPage : ContentPage
{
	public TransactionCategoriesPage(TransactionCategoriesPageViewModel vm)
	{
        InitializeComponent();

        BindingContext = vm;
    }
}