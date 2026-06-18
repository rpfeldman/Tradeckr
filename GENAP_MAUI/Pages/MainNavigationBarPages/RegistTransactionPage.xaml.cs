using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.MainNavigationBarPages;

public partial class RegistTransactionPage : ContentPage
{
	public RegistTransactionPage(RegistTransactionPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RegistTransactionPageViewModel vm)
        {
            vm.ReLoadCommand.Execute(false);
        }
    }
}