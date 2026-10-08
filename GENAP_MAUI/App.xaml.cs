using DataServices;
using GENAP_MAUI.InnerComponents;
using NetworkServices;
using Serilog;

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
                CurrencyPersistenceService currencyPersistenceService = IPlatformApplication.Current!.Services.GetRequiredService<CurrencyPersistenceService>();

                # region new user path
                // occurs when it is the first time the application is opened or when the user has not yet passed the onboarding page.
                if (GlobalResources.IsNewUser)
                {
                     Log.Debug("Advancing as new user");

                    CategoryPersistenceService categoryPersistenService = IPlatformApplication.Current!.Services.GetRequiredService<CategoryPersistenceService>();

                    var checkCategoriesOperation = await categoryPersistenService.HasCategories();
                        checkCategoriesOperation.WriteLog("Check if there are already categories in the storage");

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
                        checkCategoriesOperation.WriteLog("Check if there are Already currencies in the storage");

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
                    Log.Debug("AppLoadingResetEvent set");

                    return;
                }
                #endregion

                Application.Current?.UserAppTheme = Preferences.Get(PreferenceKeys.UserThemeKey, true) ? AppTheme.Dark : AppTheme.Light;
                 Log.Debug("UserAppTheme set");

                var lastDayEntered = Preferences.Get(PreferenceKeys.LastDayEnteredKey, DateTime.Today);

                 var getCurrenciesOperation = await currencyPersistenceService.GetAllAsync();
                     getCurrenciesOperation.WriteLog("Bring the currencies to memory");
                
                if (!getCurrenciesOperation.Success)
                {
                    System.Diagnostics.Debug.WriteLine(getCurrenciesOperation.InnerError!.ErrorMessage);
                    return;
                }

                #region application opened x+1 times path (long name lol) { x >= 1 }
                // occurs when it is not the first time the user opens the application on the same day or when the user has disabled the daily rate update
                if (lastDayEntered == DateTime.Today || !Preferences.Get(PreferenceKeys.UpdateRatesKey, true))
                {
                    GlobalResources.Currencies = [.. getCurrenciesOperation.Result!];

                    GlobalResources.AppLoadingResetEvent.Set();
                    Log.Debug("AppLoadingResetEvent set");

                    return;
                }
                #endregion

                var checkInternet = NetworkMethods.CheckInternetConnection();

                # region rates need to be updated path
                // occurs when the application is opened for the first time in the day,
                // the user has an internet connection and has enabled the feature to update rates daily
                if (checkInternet) 
                {
                    Log.Debug("NEW DAY: Updating currencies");

                    CurrenciesRatesService currenciesRatesService = new(GlobalResources.DefaultTradingCurrency.IsoCode);

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
                    Preferences.Set(PreferenceKeys.LastDayEnteredKey, DateTime.Today);

                    GlobalResources.AppLoadingResetEvent.Set();
                    Log.Debug("AppLoadingResetEvent set");

                    return; 
                }
                #endregion

                #region No internet path
                // occurs when it is necessary to update the rates but the user does not have an internet connection
                System.Diagnostics.Debug.WriteLine("User does not have internet connection. Advancing without updating the currencies rates");
                    Log.Warning("Advancing without connection");
                

                GlobalResources.Currencies = [.. getCurrenciesOperation.Result!];

                    
                 GlobalResources.AppLoadingResetEvent.Set();
                    Log.Debug("AppLoadingResetEvent set");   
                
                return;
                #endregion
            }
            catch (Exception x)
            {
                System.Diagnostics.Debug.WriteLine($"Seed failed: {x.Message}");
                    Log.Error($"Seed failed: {x.Message}");
            }
        }
    }
}