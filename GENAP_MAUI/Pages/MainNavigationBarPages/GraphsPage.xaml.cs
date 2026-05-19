using DataServices;
using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.MainNavigationBarPages;

public partial class GraphsPage : ContentPage
{
	public GraphsPage(GraphsPageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}