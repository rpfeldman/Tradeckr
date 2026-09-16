using CommunityToolkit.Maui.Views;
using Serilog;

namespace GENAP_MAUI.Pages.IntermediaryPages;

public partial class LoadingPage : ContentPage
{
	public LoadingPage()
	{
		InitializeComponent();

        Log.Debug("Loading page initialized");
	}

    private async void MediaElement_MediaEnded(object sender, EventArgs e)
    {
            Log.Debug("Media ended");

            Log.Debug("Loading page waiting AppLoadingResetEvent");
        GlobalResources.AppLoadingResetEvent.Wait();
            Log.Debug("Loading page advanced AppLoadingResetEvent");

        await Dispatcher.DispatchAsync(async () =>
        {
            if (GlobalResources.IsNewUser)
            {
                GlobalResources.AppLoadingResetEvent.Dispose();
                    Log.Debug("AppLoadingResetEvent disposed");

                    Log.Debug("Moving to OnBoardingPage");
                await Shell.Current.GoToAsync($"//{Routes.Onboarding}");
                return;
            }

            GlobalResources.AppLoadingResetEvent.Dispose();
                Log.Debug("AppLoadingResetEvent disposed");

                Log.Debug("Moving to MainDashboardPage");
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
                Log.Debug("Media started");
        });
    }

    private void SplashAnimation_MediaFailed(object sender, CommunityToolkit.Maui.Core.MediaFailedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Media failed: {e.ErrorMessage}");
            Log.Error($"Media failed: {e.ErrorMessage}");
    }
}