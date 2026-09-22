using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.ApplicationModel.Communication;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class SettingsPageViewModel : BaseViewModel
    {
        private bool _IsLoading;
        private bool[] AppearanceSettings = new bool[4];
        private bool[] CurrencySettings = new bool[4];
        private Dictionary<bool, string> UpdateRatesOptions = new(2)
        {
            { true, "Habilitado" },
            { false, "Deshabilitado" }
        };

        public List<KeyValuePair<bool, string>> UpdateRatesOptionsList { get => [.. UpdateRatesOptions];  }
       
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private partial bool Settings_HasChanged { get; set; }


        // Appearance settings properties

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        public partial string PickedUserName { get; set; }

        [ObservableProperty]
        public partial KeyValuePair<AppTheme, string> PickedTheme { get; set; }


        // Currency settings properties
        
        [ObservableProperty]
        public partial CurrencyDto PickedCommonCurrency { get; set;}

        [ObservableProperty]
        public partial KeyValuePair<bool, string> PickedUpdateRateOption { get; set; }


        // Bug report properties

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(ReportCommand))]
        public partial string BugTitle { get; set; }

        [ObservableProperty]
        public partial string BugDescription { get; set;}


        [RelayCommand(CanExecute = nameof(SaveCanExecute))]
        public async Task Save()
        {
            if (AppearanceSettings[0])
            {
                if(AppearanceSettings[1])
                {
                    Preferences.Set(PreferenceKeys.UserNameKey, PickedUserName);
                }

                if (AppearanceSettings[2])
                {
                    Application.Current?.UserAppTheme = PickedTheme.Key;

                    Preferences.Set(PreferenceKeys.UserThemeKey, PickedTheme.Key == AppTheme.Dark);
                }
            }

            if (CurrencySettings[0])
            {
                if (CurrencySettings[1])
                {
                    Preferences.Set(PreferenceKeys.CommonCurrencyKey, GlobalResources.Currencies.IndexOf(PickedCommonCurrency));
                }

                if (CurrencySettings[2])
                {
                    
                }

                if (CurrencySettings[3])
                {
                    Preferences.Set(PreferenceKeys.UpdateRatesKey, PickedUpdateRateOption.Key);
                }
            }

            for (int i = 0; i < AppearanceSettings.Length; i++) AppearanceSettings[i] = false;
            for (int i = 0; i < CurrencySettings.Length; i++)  CurrencySettings[i] = false;
            Settings_HasChanged = false;

            await Shell.Current.DisplayAlertAsync("Configuracion", "Cambios guardados con exito", "Aceptar");
        }

        [RelayCommand(CanExecute = nameof(ReportCanExecute))]
        public async Task Report()
        {
            var logPath = Path.Combine(FileSystem.AppDataDirectory, FilePaths.LogFileName);

            if (!File.Exists(logPath))
            {
                await Shell.Current.DisplayAlertAsync("Error", "No es posible reportar bugs en tu dispositivo. Por favor, contacte con soporte tecnico", "Aceptar");
                return;
            }

            string emailBody = 
                $@"
                    Date: {DateOnly.FromDateTime(DateTime.Today)}

                    A continuacion se adjuntara la informacion del dispositivo. Estos datos son claves para poder identificar el error.
                    Eres completamente libre de eliminar los datos que no quieras enviar.

                    Platform: {DeviceInfo.Platform} 
                    Manufacter: {DeviceInfo.Manufacturer}
                    Model: {DeviceInfo.Model}
                    OS Version: {DeviceInfo.VersionString}
                    Device idiom: {DeviceInfo.Idiom}
                    Device type: {DeviceInfo.DeviceType}
                " + Environment.NewLine + BugDescription;

            EmailMessage emailMessage = new(BugTitle, emailBody, ["ramirofeldman0@gmail.com"]); // TEMPORARY MAIL
                emailMessage.Attachments?.Add(new EmailAttachment(logPath));

            await Email.Default.ComposeAsync(emailMessage);
        }

        [RelayCommand]
        public async Task Load()
        {
            _IsLoading = true;

            PickedUserName = GlobalResources.UserName;
            PickedTheme = Preferences.Get(PreferenceKeys.UserThemeKey, true) ? GlobalResources.AppThemesList[0] : GlobalResources.AppThemesList[1];

            PickedCommonCurrency = GlobalResources.DefaultCommonCurrency;
            PickedUpdateRateOption = Preferences.Get(PreferenceKeys.UpdateRatesKey, true) ? UpdateRatesOptionsList[0] : UpdateRatesOptionsList[1];
            

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

        partial void OnPickedThemeChanged(KeyValuePair<AppTheme, string> value)
        {
           if(_IsLoading) { return; }

           AppearanceSettings[0] = true;
           AppearanceSettings[2] = true;

           Settings_HasChanged = true;
        }

        partial void OnPickedCommonCurrencyChanged(CurrencyDto value)
        {
           if(_IsLoading) { return; }

           CurrencySettings[0] = true;
           CurrencySettings[1] = true;

           Settings_HasChanged = true;
        }

        partial void OnPickedUpdateRateOptionChanged(KeyValuePair<bool, string> value)
        {
           if(_IsLoading) { return; }

           CurrencySettings[0] = true;
           CurrencySettings[3] = true;

           Settings_HasChanged = true;
        }

        private bool SaveCanExecute() => Settings_HasChanged && !string.IsNullOrWhiteSpace(PickedUserName) && PickedUserName.Length < 20;
        private bool ReportCanExecute() => !string.IsNullOrWhiteSpace(BugTitle);
    }
}
