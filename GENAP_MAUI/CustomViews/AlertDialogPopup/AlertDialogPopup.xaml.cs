namespace GENAP_MAUI.CustomViews.AlertDialogPopup;

public partial class AlertDialogPopup : ContentView
{
	public AlertDialogPopup(AlertDialogPopupViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}