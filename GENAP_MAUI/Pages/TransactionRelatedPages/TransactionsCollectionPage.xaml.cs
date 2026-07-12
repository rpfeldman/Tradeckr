using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.TransactionRelatedPages;

public partial class TransactionsCollectionPage : ContentPage
{
	public TransactionsCollectionPage(TransactionsCollectionPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TransactionsCollectionPageViewModel vm)
        {
            await vm.ReLoadCommand.ExecuteAsync(false);
        }
    }
}