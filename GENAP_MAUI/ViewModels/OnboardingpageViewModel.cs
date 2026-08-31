using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using static GENAP_MAUI.GlobalResources;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class OnboardingpageViewModel : BaseViewModel
    {
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
            Preferences.Set(PreferenceKeys.NewUserKey, false);
            Preferences.Set(PreferenceKeys.UserNameKey, UserName);
            Preferences.Set(PreferenceKeys.CommonCurrencyKey, GlobalResources.Currencies.IndexOf(PickedCommonCurrency));
            Preferences.Set(PreferenceKeys.TradingCurrencyKey, GlobalResources.Currencies.IndexOf(PickedTradingCurrency));

            await DirectNavigate(Routes.Dashboard);
        }

        private bool ContinueCanExecute() => !string.IsNullOrWhiteSpace(UserName) && UserName.Length < 20;
    }
}
