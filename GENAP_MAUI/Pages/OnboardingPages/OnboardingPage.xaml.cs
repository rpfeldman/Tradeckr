using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI.Pages.OnboardingPages;

public partial class OnboardingPage : ContentPage
{
	public OnboardingPage(OnboardingpageViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is OnboardingpageViewModel vm)
        {
            await vm.PingerCommand.ExecuteAsync(false);
        }
    }
}