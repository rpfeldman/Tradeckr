using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DomainModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
            Log.Information("Settings saved (Settings Page)");

            if (AppearanceSettings[0])
            {
                if(AppearanceSettings[1])
                {
                    Log.Information($"Tried to change Username from '{GlobalResources.UserName}' to '{PickedUserName}' (Settings page)");

                    Preferences.Set(PreferenceKeys.UserNameKey, PickedUserName);
                }

                if (AppearanceSettings[2])
                {
                    Application.Current?.UserAppTheme = PickedTheme.Key;

                    Preferences.Set(PreferenceKeys.UserThemeKey, PickedTheme.Key == AppTheme.Dark);
                }

                if (AppearanceSettings[3])
                {
                    // TO - DO
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
                    // TO DO
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
            if(DeviceInfo.Idiom == DeviceIdiom.Desktop)
            {
                await Shell.Current.DisplayAlertAsync("Error", "El reporte de errores desde la app es exclusivo de dispositivos móviles.\n\nPuedes hacer tu reporte enviando el mismo a 'ramirofeldman0@gmail.com'", "Aceptar");
                return;
            }

            var logPath = Path.Combine(FileSystem.AppDataDirectory, FilePaths.LogFileName);

            if (!File.Exists(logPath))
            {
                await Shell.Current.DisplayAlertAsync("Error", "No es posible reportar bugs en tu dispositivo.\nPor favor, contacte con soporte tecnico", "Aceptar");
                return;
            }

            string bugAndDeviceInfo = 
                $"Bug title: {BugTitle}\n"+
                $"Bug description: {BugDescription}\n\n"+
                $"Date: {DateOnly.FromDateTime(DateTime.Today)}\n\n" +
                $"A continuación se adjuntará la información del dispositivo. "+
                $"Estos datos son claves para poder identificar el error. " +
                $"Eres completamente libre de eliminar los datos que no quieras enviar."+
                $"\n -Platform: {DeviceInfo.Platform}" +
                $"\n -Manufacturer: {DeviceInfo.Manufacturer}" +
                $"\n -Model: {DeviceInfo.Model}" +
                $"\n -OS Version: {DeviceInfo.VersionString}" +
                $"\n -Device idiom: {DeviceInfo.Idiom}" +
                $"\n -Device type: {DeviceInfo.DeviceType}" +
                $"\n -Culture: {CultureInfo.CurrentCulture}\n\n"; 
                                
                                
            string applicationLog = await File.ReadAllTextAsync(logPath);

            var bugReportPath = Path.Combine(FileSystem.AppDataDirectory, "BugReport.txt");

            await File.WriteAllTextAsync(bugReportPath, bugAndDeviceInfo + applicationLog);

            if (!Email.Default.IsComposeSupported)
            {
                Log.Warning("Email is not supported. Advancing with file sharing (bug report)");
                
                await Shell.Current.DisplayAlertAsync("Reporte", "Aparentemente no tienes una aplicacion predeterminada de correo electronico en tu dispositivo.\n\nPor favor, intente compartir el siguiente archivo de texto a 'ramirofeldman0@gmail.com'","Aceptar");


                ShareFile file = new(bugReportPath);
                ShareFileRequest request = new("Bug", file);
                await Share.Default.RequestAsync(request);

               await Shell.Current.DisplayAlertAsync($"Gracias, {GlobalResources.UserName}", "El equipo de Tradeckr agradece infinitamente que te hayas tomado el tiempo de realizar tu reporte\n\n¡Tu colaboración nos ayuda a ser mejores cada día!","Aceptar");

                return;
            }

            EmailMessage emailMessage = new(BugTitle, BugDescription, ["ramirofeldman0@gmail.com"]); // TEMPORARY MAIL
            var attachment = new EmailAttachment(bugReportPath);

           emailMessage.Attachments ??= [];
           emailMessage.Attachments.Add(attachment);

            await Email.Default.ComposeAsync(emailMessage);

            await Shell.Current.DisplayAlertAsync($"Gracias, {GlobalResources.UserName}", "El equipo de Tradeckr agradece infinitamente que te hayas tomado el tiempo de realizar tu reporte\n\n¡Tu colaboración nos ayuda a ser mejores cada día!", "Aceptar");
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
