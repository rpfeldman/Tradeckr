using CommunityToolkit.Mvvm.Input;
using DataServices;
using GENAP_MAUI.InnerComponents;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class ClearStorageConfirmationPageViewModel(DataManagementService dataManagementService, CategoryPersistenceService categoryPersistenceService) : BaseViewModel
    {
        private DataManagementService _dataManagementService = dataManagementService;
        private CategoryPersistenceService _categoryPersistenceService = categoryPersistenceService;

        [RelayCommand]
        public async Task ClearStorage()
        {
            var alert = await Shell.Current.DisplayAlertAsync("Confirmar", "¿desea confirmar el reinicio de los datos?", "Si, reinciar", "No cancelar");

            if (!alert){ return; }

            var clearStorageOperation = await _dataManagementService.RestartDataAsync();
                clearStorageOperation.WriteLog("Remove all transactions in the storage (Clear Storage)");

            if (!clearStorageOperation.Success)
            {
                await Shell.Current.DisplayAlertAsync("Error", clearStorageOperation.InnerError?.ErrorMessage, "Aceptar");
                return;
            }

             var clearCategoriesOperation = await _categoryPersistenceService.RestartDataAsync();
                clearCategoriesOperation.WriteLog("Remove all categories in the storage (Clear Storage)");

             if (!clearCategoriesOperation.Success)
             {
                await Shell.Current.DisplayAlertAsync("Error", clearCategoriesOperation.InnerError?.ErrorMessage, "Aceptar");
                return;
             }

             var addDefaultCategoriesOperation = await _categoryPersistenceService.AddCategoriesAsync(DefaultCategories.DefaultCategoriesList);
                addDefaultCategoriesOperation.WriteLog("Add the default categories in the storage (Clear Storage)");

             if (!addDefaultCategoriesOperation.Success)
             {
                await Shell.Current.DisplayAlertAsync("Error", addDefaultCategoriesOperation.InnerError?.ErrorMessage, "Aceptar");
                return;
             }

            await Shell.Current.DisplayAlertAsync("Exito", "Se han reiniciado los datos correctamente\n\nVolviendo al menu...", "aceptar");
            await DirectNavigate(Routes.Dashboard);
        }
    }
}
