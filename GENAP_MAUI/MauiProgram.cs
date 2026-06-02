using GENAP_MAUI.Pages.MainNavigationBarPages;
using Microsoft.Extensions.Logging;
using GENAP_MAUI.ViewModels;
using DataServices;
using Repositories;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LiveChartsCore.SkiaSharpView.Maui;

namespace GENAP_MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp()
                .UseLiveCharts()
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
            builder.Services.AddTransient<RegistTradeTransactionPageViewModel>();
            builder.Services.AddTransient<TransactionCategoriesPageViewModel>();
            builder.Services.AddTransient<GraphsPageViewModel>();
            builder.Services.AddTransient<TransactionsCollectionPageViewModel>();
            builder.Services.AddTransient<TransactionPageViewModel>();

            // Data services & the repository
            builder.Services.AddSingleton<IStateStorage, EF_SQLite_StateStorage>(sp => { return new EF_SQLite_StateStorage("TermporalTest.db", [14, 2]); });
            builder.Services.AddSingleton<DataRegistrationService>();
            builder.Services.AddSingleton<DataProjectionService>();
            builder.Services.AddSingleton<DataManagementService>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
