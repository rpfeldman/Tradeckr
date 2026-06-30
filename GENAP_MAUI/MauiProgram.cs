using GENAP_MAUI.Pages.MainNavigationBarPages;
using Microsoft.Extensions.Logging;
using GENAP_MAUI.ViewModels;
using DataServices;
using Repositories;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LiveChartsCore.SkiaSharpView.Maui;
using DomainModel;
using GENAP_MAUI.InnerServices;

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
                    fonts.AddFont("Symbols.ttf", "Symbols");
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
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Test.db");
            builder.Services.AddSingleton<IStateStorage<TransactionDto>, EF_SQLite_StateStorageRepo<TransactionDto>>(sp => { return new EF_SQLite_StateStorageRepo<TransactionDto>(dbPath); });

            builder.Services.AddSingleton<DataRegistrationService>();
            builder.Services.AddSingleton<DataProjectionService>();
            builder.Services.AddSingleton<DataManagementService>();

            // inner services
            builder.Services.AddSingleton<CategoriesPersistenceService>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
