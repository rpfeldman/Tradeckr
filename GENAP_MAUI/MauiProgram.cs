using GENAP_MAUI.Pages.MainNavigationBarPages;
using Microsoft.Extensions.Logging;
using GENAP_MAUI.ViewModels;
using DataServices;
using Repositories;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LiveChartsCore.SkiaSharpView.Maui;
using DomainModel;
using SQLitePCL;
using NetworkServices;
using CommunityToolkit.Maui;
using Serilog;

namespace GENAP_MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Batteries_V2.Init();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp()
                .UseLiveCharts()
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement(false)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Symbols.ttf", "Symbols");
                });

            // Paths
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, FilePaths.StorageFileName);
            var logPath = Path.Combine(FileSystem.AppDataDirectory, FilePaths.LogFileName);

            // Logging system
            Log.Logger = new LoggerConfiguration().WriteTo.File(logPath, shared: true).MinimumLevel.Debug().CreateLogger();

            // ViewModels
            builder.Services.AddTransient<MainDashboardPageViewModel>();
            builder.Services.AddTransient<RegistTransactionPageViewModel>();
            builder.Services.AddTransient<RegistTradeTransactionPageViewModel>();
            builder.Services.AddTransient<TransactionCategoriesPageViewModel>();
            builder.Services.AddTransient<GraphsPageViewModel>();
            builder.Services.AddTransient<TransactionsCollectionPageViewModel>();
            builder.Services.AddTransient<TransactionPageViewModel>();
            builder.Services.AddTransient<ClearStorageConfirmationPageViewModel>();
            builder.Services.AddTransient<OnboardingpageViewModel>();
            builder.Services.AddTransient<SettingsPageViewModel>();

            // Data services & the repository 
            builder.Services.AddSingleton<IStateStorage<TransactionDto>, EF_SQLite_StateStorageRepo<TransactionDto>>(sp => { return new EF_SQLite_StateStorageRepo<TransactionDto>(dbPath); });

            builder.Services.AddSingleton<DataRegistrationService>();
            builder.Services.AddSingleton<DataProjectionService>();
            builder.Services.AddSingleton<DataManagementService>();

            // Category persistence service & repository
            builder.Services.AddSingleton<IStateStorage<CategoryDto>, EF_SQLite_StateStorageRepo<CategoryDto>>(sp => { return new EF_SQLite_StateStorageRepo<CategoryDto>(dbPath); });
            builder.Services.AddSingleton<CategoryPersistenceService>();

            // Currencies services & repository
            builder.Services.AddSingleton<IStateStorage<CurrencyDto>, EF_SQLite_StateStorageRepo<CurrencyDto>>(sp => { return new EF_SQLite_StateStorageRepo<CurrencyDto>(dbPath); });
            builder.Services.AddSingleton<CurrencyPersistenceService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
