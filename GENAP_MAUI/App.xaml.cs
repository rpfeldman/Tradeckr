using DataServices;
using NetworkServices;
using Microsoft.Extensions.DependencyInjection;
using DomainModel;

namespace GENAP_MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
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
                    CategoryPersistenceService categoryPersistenService = IPlatformApplication.Current!.Services.GetRequiredService<CategoryPersistenceService>();
                    

                    var checkCategoriesOperation = await categoryPersistenService.HasCategories();

                    if (!checkCategoriesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(checkCategoriesOperation.InnerError?.ErrorMessage);
                        return;
                    }

                    if (!checkCategoriesOperation.Result)
                    {
                        var setDefaultCategoriesOperation = await categoryPersistenService.AddCategoriesAsync(DefaultCategories.DefaultCategoriesList);

                        if (!setDefaultCategoriesOperation.Success)
                        {
                            System.Diagnostics.Debug.WriteLine(setDefaultCategoriesOperation.InnerError?.ErrorMessage);
                            return;
                        }
                    }
                    var checkCurrenciesOperation = await currencyPersistenceService.HasCurrencies();

                    if (!checkCurrenciesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(checkCurrenciesOperation.InnerError?.ErrorMessage);
                    }

                    if (!checkCurrenciesOperation.Result)
                    {
                        var setCurrenciesOperation = await currencyPersistenceService.AddRangeAsync(GlobalResources.Currencies);

                        if (!setCurrenciesOperation.Success)
                        {
                            System.Diagnostics.Debug.WriteLine(setCurrenciesOperation.InnerError?.ErrorMessage);
                            return;
                        }
                    }

                    return;
                }

                Application.Current?.UserAppTheme = Preferences.Get(PreferenceKeys.UserThemeKey, true) ? AppTheme.Dark : AppTheme.Light;

                if (!NetworkMethods.CheckInternetConnection()) 
                { 
                    System.Diagnostics.Debug.WriteLine("User does not have internet connection. Advacing without updating the currencies rates");

                    var getCurrenciesOperation = await currencyPersistenceService.GetAllAsync();

                    if (!getCurrenciesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(getCurrenciesOperation.InnerError!.ErrorMessage);
                        return;
                    }

                    GlobalResources.Currencies = [.. getCurrenciesOperation.Result!];

                    return; 
                }

                var lastDayEntered = Preferences.Get(PreferenceKeys.LastDayEnteredKey, DateTime.Today);

                if(lastDayEntered != DateTime.Today)
                {
                    CurrenciesRatesService currenciesRatesService = new(GlobalResources.DefaultTradingCurrency.IsoCode);

                    var getCurrenciesOperation = await currencyPersistenceService.GetAllAsync();

                    if (!getCurrenciesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(getCurrenciesOperation.InnerError!.ErrorMessage);
                        return;
                    }

                    var currencies = getCurrenciesOperation.Result!.ToArray();

                    var updateRatesOperation = await currenciesRatesService.UpdateCurrenciesRatesAsync(currencies, DateOnly.FromDateTime(DateTime.Today));
                    
                    if(!updateRatesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(updateRatesOperation.InnerError!.ErrorMessage);
                        return;
                    }

                    GlobalResources.Currencies = currencies;

                    var updateCurrenciesOperation = await currencyPersistenceService.UpdateRangeAsync(currencies);
                    if (!updateRatesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(updateCurrenciesOperation.InnerError!.ErrorMessage);
                        return;
                    }
                }
                else
                {
                    var getCurrenciesOperation = await currencyPersistenceService.GetAllAsync();

                    if (!getCurrenciesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(getCurrenciesOperation.InnerError!.ErrorMessage);
                        return;
                    }

                    GlobalResources.Currencies = [.. getCurrenciesOperation.Result!];
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