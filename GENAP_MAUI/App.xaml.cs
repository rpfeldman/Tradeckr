using DataServices;
using NetworkServices;
using Microsoft.Extensions.DependencyInjection;
using DomainModel;
using Serilog;
using GENAP_MAUI.InnerComponents;

namespace GENAP_MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Log.Debug("App initialized");
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                // TO - DO: Apply a log system

                CurrencyPersistenceService currencyPersistenceService = IPlatformApplication.Current!.Services.GetRequiredService<CurrencyPersistenceService>();

                if (GlobalResources.IsNewUser)
                {
                     Log.Debug("Advacing as new user");

                    CategoryPersistenceService categoryPersistenService = IPlatformApplication.Current!.Services.GetRequiredService<CategoryPersistenceService>();

                    var checkCategoriesOperation = await categoryPersistenService.HasCategories();
                        checkCategoriesOperation.WriteLog("Check if there are alredy categories in the storage");

                    if (!checkCategoriesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(checkCategoriesOperation.InnerError?.ErrorMessage);
                        return;
                    }

                    if (!checkCategoriesOperation.Result)
                    {
                        var setDefaultCategoriesOperation = await categoryPersistenService.AddCategoriesAsync(DefaultCategories.DefaultCategoriesList);
                            setDefaultCategoriesOperation.WriteLog("Set and save the default categories in the storage");

                        if (!setDefaultCategoriesOperation.Success)
                        {
                            System.Diagnostics.Debug.WriteLine(setDefaultCategoriesOperation.InnerError?.ErrorMessage);
                            return;
                        }
                    }
                    var checkCurrenciesOperation = await currencyPersistenceService.HasCurrencies();
                        checkCategoriesOperation.WriteLog("Check if there are alredy currencies in the storage");

                    if (!checkCurrenciesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(checkCurrenciesOperation.InnerError?.ErrorMessage);
                    }

                    if (!checkCurrenciesOperation.Result)
                    {
                        var setCurrenciesOperation = await currencyPersistenceService.AddRangeAsync(GlobalResources.Currencies);
                            setCurrenciesOperation.WriteLog("Set and save the default currencies in the storage");

                        if (!setCurrenciesOperation.Success)
                        {
                            System.Diagnostics.Debug.WriteLine(setCurrenciesOperation.InnerError?.ErrorMessage);
                            return;
                        }
                    }

                    GlobalResources.AppLoadingResetEvent.Set();
                    Log.Debug("AppLoadingResetEvent setted");

                    return;
                }

                Application.Current?.UserAppTheme = Preferences.Get(PreferenceKeys.UserThemeKey, true) ? AppTheme.Dark : AppTheme.Light;
                 Log.Debug("UserAppTheme setted");

                if (!NetworkMethods.CheckInternetConnection()) 
                { 
                    System.Diagnostics.Debug.WriteLine("User does not have internet connection. Advacing without updating the currencies rates");
                    Log.Warning("Advacing without connection");

                    var getCurrenciesOperation = await currencyPersistenceService.GetAllAsync();
                        getCurrenciesOperation.WriteLog("Bring to memory the currencies in the storage");

                    if (!getCurrenciesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(getCurrenciesOperation.InnerError!.ErrorMessage);
                        return;
                    }

                    GlobalResources.Currencies = [.. getCurrenciesOperation.Result!];

                    
                    GlobalResources.AppLoadingResetEvent.Set();
                    Log.Debug("AppLoadingResetEvent setted");

                    return; 
                }

                var lastDayEntered = Preferences.Get(PreferenceKeys.LastDayEnteredKey, DateTime.Today);

                if(lastDayEntered != DateTime.Today)
                {
                    Log.Debug("NEW DAY: Updating currencies");

                    CurrenciesRatesService currenciesRatesService = new(GlobalResources.DefaultTradingCurrency.IsoCode);

                    var getCurrenciesOperation = await currencyPersistenceService.GetAllAsync();
                        getCurrenciesOperation.WriteLog("Bring to memory the currencies in the storage");

                    if (!getCurrenciesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(getCurrenciesOperation.InnerError!.ErrorMessage);
                        return;
                    }

                    var currencies = getCurrenciesOperation.Result!.ToArray();

                    var updateRatesOperation = await currenciesRatesService.UpdateCurrenciesRatesAsync(currencies, DateOnly.FromDateTime(DateTime.Today));
                        updateRatesOperation.WriteLog($"Update the currencies rates");
                    
                    if(!updateRatesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(updateRatesOperation.InnerError!.ErrorMessage);
                        return;
                    }

                    GlobalResources.Currencies = currencies;

                    var updateCurrenciesOperation = await currencyPersistenceService.UpdateRangeAsync(currencies);
                        updateCurrenciesOperation.WriteLog("Update the currencies rates in the storage");

                    if (!updateRatesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(updateCurrenciesOperation.InnerError!.ErrorMessage);
                        return;
                    }

                    Preferences.Set(PreferenceKeys.LastRateUpdateKey, DateTime.Today);

                    GlobalResources.AppLoadingResetEvent.Set();
                    Log.Debug("AppLoadingResetEvent setted");
                }
                else
                {
                    var getCurrenciesOperation = await currencyPersistenceService.GetAllAsync();
                        getCurrenciesOperation.WriteLog("Bring the currencies to memory");

                    if (!getCurrenciesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(getCurrenciesOperation.InnerError!.ErrorMessage);
                        return;
                    }

                    GlobalResources.Currencies = [.. getCurrenciesOperation.Result!];

                    GlobalResources.AppLoadingResetEvent.Set();
                    Log.Debug("AppLoadingResetEvent setted");
                }

                Preferences.Set(PreferenceKeys.LastDayEnteredKey, DateTime.Today);
            }
            catch (Exception x)
            {
                System.Diagnostics.Debug.WriteLine($"Seed failed: {x.Message}");
            }
        }
    }
}