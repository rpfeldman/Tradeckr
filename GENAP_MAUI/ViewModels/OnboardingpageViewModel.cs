
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

        private bool IsPingerActive { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        public partial bool IsConnected { get; set; }

        [ObservableProperty]
        public partial bool InlineWarningVisibily { get; set; }

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
            Preferences.Set(PreferenceKeys.LastRateUpdateKey, DateTime.Today);

            IsPingerActive = false;
            await DirectNavigate(Routes.Dashboard);
        }

        [RelayCommand]
        public async Task Pinger()
        {
            IsPingerActive = true;

            while (IsPingerActive)
            { 
                IsConnected = NetworkMethods.CheckInternetConnection();
                InlineWarningVisibily = !IsConnected;

                System.Diagnostics.Debug.WriteLine("Ping: " + (IsConnected ? "correctly connected" : "the connection could not be established")); 

                await Task.Delay(3500);
            }
        }

        private bool ContinueCanExecute() => !string.IsNullOrWhiteSpace(UserName) && UserName.Length < 20 && IsConnected;
    }
}
