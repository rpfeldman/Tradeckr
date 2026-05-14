using GENAP_MAUI.Pages.MainNavigationBarPages;
using Microsoft.Extensions.Logging;
using GENAP_MAUI.ViewModels;

namespace GENAP_MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Global resources
            builder.Services.AddSingleton<GlobalResources>();

            // ViewModels
            builder.Services.AddTransient<MainDashboardPageViewModel>();
            builder.Services.AddTransient<RegistTransactionPageViewModel>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
