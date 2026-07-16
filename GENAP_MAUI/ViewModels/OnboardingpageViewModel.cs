using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class OnboardingpageViewModel : BaseViewModel
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ContinueCommand))]
        public partial string UserName { get; set; }

        [RelayCommand(CanExecute = nameof(ContinueCanExecute))]
        public async Task Continue()
        {
            Preferences.Set(PreferenceKeys.NewUserKey, false);
            Preferences.Set(PreferenceKeys.UserNameKey, UserName);

            await DirectNavigate(Routes.Dashboard);
        }

        private bool ContinueCanExecute() => !string.IsNullOrWhiteSpace(UserName) && UserName.Length < 20;
    }
}
