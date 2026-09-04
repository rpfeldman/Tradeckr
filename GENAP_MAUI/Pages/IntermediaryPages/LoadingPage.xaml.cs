using CommunityToolkit.Maui.Views;

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

        await Dispatcher.DispatchAsync(async () =>
        {
            if (GlobalResources.IsNewUser)
            {
                await Shell.Current.GoToAsync($"//{Routes.Onboarding}");
                return;
            }

            await Shell.Current.GoToAsync($"//{Routes.Dashboard}");
        });
    }

    private void SplashAnimation_MediaOpened(object sender, EventArgs e)
    {
        if (sender is not MediaElement mediaElement)
            return;

        mediaElement.Dispatcher.Dispatch(() =>
        {
            mediaElement.Play();
        });
    }

    private void SplashAnimation_MediaFailed(object sender, CommunityToolkit.Maui.Core.MediaFailedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Media failed: {e.ErrorMessage}");
    }
}