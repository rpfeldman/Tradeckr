using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.MainNavigationBarPages;

public partial class TransactionCategoriesPage : ContentPage
{
	public TransactionCategoriesPage(TransactionCategoriesPageViewModel vm)
	{
		try
		{
            InitializeComponent();

            BindingContext = vm;
        }
		catch (Exception x)
		{
			Shell.Current.DisplayAlertAsync("DEBUG: error", $"{x.Message}\n{x.Data}\n{x.InnerException}\n{x.StackTrace}\n{x.Source}", "pija");
		}
		
    }
}