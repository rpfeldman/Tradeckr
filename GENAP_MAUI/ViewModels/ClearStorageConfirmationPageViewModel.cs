using CommunityToolkit.Mvvm.Input;
using DataServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class ClearStorageConfirmationPageViewModel(DataManagementService dataManagementService) : BaseViewModel
    {
        private DataManagementService _dataManagementService = dataManagementService;

        [RelayCommand]
        public async Task ClearStorage()
        {
            var alert = await Shell.Current.DisplayAlertAsync("Confirmar", "¿desea confirmar el reinicio de los datos?", "Si, reinciar", "No cancelar");

            if (!alert){ return; }

            var clearStorageOperation = await _dataManagementService.RestartDataAsync();

            if (!clearStorageOperation.Success)
            {
                await Shell.Current.DisplayAlertAsync("Error", clearStorageOperation.InnerError?.ErrorMessage, "Aceptar");
                return;
            }

            await Shell.Current.DisplayAlertAsync("Exito", "Se han reiniciado los datos correctamente\n\nVolviendo al menu...", "aceptar");
            await DirectNavigate(Routes.Dashboard);
        }
    }
}
