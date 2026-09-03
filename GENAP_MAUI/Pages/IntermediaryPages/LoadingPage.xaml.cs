namespace GENAP_MAUI.Pages.IntermediaryPages;

public partial class LoadingPage : ContentPage
{
	public LoadingPage()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(1000); // this is temporary 

        if (GlobalResources.IsNewUser)
        {
            await Shell.Current.GoToAsync($"//{Routes.Onboarding}");
            return;
        }

        await Shell.Current.GoToAsync($"//{Routes.Dashboard}");
    }
}