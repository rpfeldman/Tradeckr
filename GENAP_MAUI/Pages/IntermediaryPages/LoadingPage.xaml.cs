namespace GENAP_MAUI.Pages.IntermediaryPages;

public partial class LoadingPage : ContentPage
{
	public LoadingPage()
	{
		InitializeComponent();
	}

    private async void MediaElement_MediaEnded(object sender, EventArgs e)
    {
        // TO-DO an awaiter for the app.xaml.cs

        if (GlobalResources.IsNewUser)
        {
            await Shell.Current.GoToAsync($"//{Routes.Onboarding}");
            return;
        }

        await Shell.Current.GoToAsync($"//{Routes.Dashboard}");
    }
}