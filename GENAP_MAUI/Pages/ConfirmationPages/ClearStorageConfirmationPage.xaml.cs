using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.ConfirmationPages;

public partial class ClearStorageConfirmationPage : ContentPage
{
	public ClearStorageConfirmationPage(ClearStorageConfirmationPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}