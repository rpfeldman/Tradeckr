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
        private bool[] AppearanceSettings = new bool[4];
        private bool[] CurrencySettings = new bool[4];

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private partial bool Settings_HasChanged { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        public partial string PickedUserName { get; set; }

        [RelayCommand(CanExecute = nameof(SaveCanExecute))]
        public async Task Save()
        {
            if (AppearanceSettings[0])
            {
                if(AppearanceSettings[1])
                {
                    Preferences.Set(PreferenceKeys.UserNameKey, PickedUserName);
                }
                
            }

            if (CurrencySettings[0])
            {

            }

            for (int i = 0; i < AppearanceSettings.Length; i++) AppearanceSettings[i] = false;
            for (int i = 0; i < CurrencySettings.Length; i++)  CurrencySettings[i] = false;
            Settings_HasChanged = false;

            await Shell.Current.DisplayAlertAsync("Configuracion", "Cambios guardados con exito", "Aceptar");
        }

        [RelayCommand]
        public async Task Load()
        {
            _IsLoading = true;

            PickedUserName = GlobalResources.UserName;

            for (int i = 0; i < AppearanceSettings.Length; i++) AppearanceSettings[i] = false;
            for (int i = 0; i < CurrencySettings.Length; i++)  CurrencySettings[i] = false;
            Settings_HasChanged = false;

            _IsLoading = false;
        }

        partial void OnPickedUserNameChanged(string value)
        {
           if(_IsLoading) { return; }

           AppearanceSettings[0] = true;
           AppearanceSettings[1] = true;

           Settings_HasChanged = true;
        }

        private bool SaveCanExecute() => Settings_HasChanged && !string.IsNullOrWhiteSpace(PickedUserName) && PickedUserName.Length < 20;
    }
}
