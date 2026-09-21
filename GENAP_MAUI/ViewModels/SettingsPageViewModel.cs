using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class SettingsPageViewModel : BaseViewModel
    {
        private bool _IsLoading;
        private bool AppearanceSettings_HasChanged;
        private bool CurrencySettings_HasChanged;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private partial bool Settings_HasChanged { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        public partial string PickedUserName { get; set; }

        [RelayCommand(CanExecute = nameof(SaveCanExecute))]
        public async Task Save()
        {
            if (AppearanceSettings_HasChanged)
            {
                Preferences.Set(PreferenceKeys.UserNameKey, PickedUserName);
            }

            if (CurrencySettings_HasChanged)
            {

            }

            await Shell.Current.DisplayAlertAsync("Configuracion", "Cambios guardados con exito", "Aceptar");
        }

        [RelayCommand]
        public async Task Load()
        {
            _IsLoading = true;

            PickedUserName = GlobalResources.UserName;

            AppearanceSettings_HasChanged = false;
            CurrencySettings_HasChanged = false;
            Settings_HasChanged = false;

            _IsLoading = false;
        }

        partial void OnPickedUserNameChanged(string value)
        {
           if(_IsLoading) { return; }
           
           AppearanceSettings_HasChanged = true;
           Settings_HasChanged = true;
        }

        private bool SaveCanExecute() => Settings_HasChanged && !string.IsNullOrWhiteSpace(PickedUserName) && PickedUserName.Length < 20;
    }
}
