namespace GENAP_MAUI.CustomViews.DoubleActionDialogPopup;

public partial class DoubleActionDialogPopup : ContentView
{
	public DoubleActionDialogPopup(DoubleActionDialogPopupViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}