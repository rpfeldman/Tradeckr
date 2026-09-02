
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using NetworkServices;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static GENAP_MAUI.GlobalResources;


namespace GENAP_MAUI.ViewModels
{
    public sealed partial class OnboardingpageViewModel(CurrencyPersistenceService currencyPersistenceService) : BaseViewModel
    {
        private CurrencyPersistenceService _CurrencyPersistenceService = currencyPersistenceService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        public partial string UserName { get; set; }

        [ObservableProperty]
        public partial CurrencyDto PickedCommonCurrency { get; set; } = GlobalResources.Currencies.First();

        [ObservableProperty]
        public partial CurrencyDto PickedTradingCurrency { get; set; } = GlobalResources.Currencies.First();

        [RelayCommand(CanExecute = nameof(ContinueCanExecute))]
        public async Task Continue()
        {
            Preferences.Set(PreferenceKeys.UserNameKey, UserName);
            Preferences.Set(PreferenceKeys.CommonCurrencyKey, GlobalResources.Currencies.IndexOf(PickedCommonCurrency));
            Preferences.Set(PreferenceKeys.TradingCurrencyKey, GlobalResources.Currencies.IndexOf(PickedTradingCurrency));
            Preferences.Set(PreferenceKeys.LastDayEnteredKey, DateTime.Today);
            Preferences.Set(PreferenceKeys.UserThemeKey, Application.Current?.RequestedTheme == AppTheme.Dark); 

            // I had to force users to use the dark theme as the default theme
            // because at the first start of the application the theme won't be 'dark' or 'light'
            // for some reason it will be 'unspecified'
            // so it's impossible to know which theme the user is really using on their device.
            Preferences.Set(PreferenceKeys.UserThemeKey, Application.Current?.RequestedTheme == AppTheme.Dark); 

            var currenciesRatesService = new CurrenciesRatesService(PickedTradingCurrency.IsoCode);

            var updateCurrenciesRatesOperation = await currenciesRatesService.UpdateCurrenciesRatesAsync(GlobalResources.Currencies, DateOnly.FromDateTime(DateTime.Today));

            if (!updateCurrenciesRatesOperation.Success)
            {
                await Shell.Current.DisplayAlertAsync("Error", updateCurrenciesRatesOperation.InnerError!.ErrorMessage, "Aceptar");
                return;
            }

            var saveCurrenciesOperation = await _CurrencyPersistenceService.UpdateRangeAsync(GlobalResources.Currencies);

            if (!saveCurrenciesOperation.Success)
            {
                await Shell.Current.DisplayAlertAsync("Error", saveCurrenciesOperation.InnerError!.ErrorMessage, "Aceptar");
                return;
            }

            Preferences.Set(PreferenceKeys.NewUserKey, false);
            await DirectNavigate(Routes.Dashboard);
        }

        private bool ContinueCanExecute() => !string.IsNullOrWhiteSpace(UserName) && UserName.Length < 20;
    }
}
