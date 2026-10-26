using CommunityToolkit.Mvvm.Input;
using DataServices;
using GENAP_MAUI.CustomViews;
using GENAP_MAUI.InnerComponents;
using GENAP_MAUI.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class ClearStorageConfirmationPageViewModel(DataManagementService dataManagementService, CategoryPersistenceService categoryPersistenceService, CustomDialogsService customDialogsService) : BaseViewModel
    {
        private DataManagementService _dataManagementService = dataManagementService;
        private CategoryPersistenceService _categoryPersistenceService = categoryPersistenceService;

        private CustomDialogsService _customDialogsService = customDialogsService;

        [RelayCommand]
        public async Task ClearStorage()
        {
            var alert = await _customDialogsService.ShowDoubleActionDialogAsync
                (
                    AppResources.ConfirmClearStorage_AlertTitle, 
                    AppResources.ConfirmClearStorage_AlertContent, 
                    AppResources.ConfirmClearStorage_ClearStorageOption, 
                    AppResources.ConfirmClearStorage_CancelOption
                );

            if (alert == 2 || alert == 0){ return; }

            var clearStorageOperation = await _dataManagementService.RestartDataAsync();
                clearStorageOperation.WriteLog("Remove all transactions in the storage (Clear Storage)");

            if (!clearStorageOperation.Success)
            {
                await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, clearStorageOperation.InnerError?.ErrorMessage!, AppResources.Accept_AlertButton);
                return;
            }

             var clearCategoriesOperation = await _categoryPersistenceService.RestartDataAsync();
                clearCategoriesOperation.WriteLog("Remove all categories in the storage (Clear Storage)");

             if (!clearCategoriesOperation.Success)
             {
                await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, clearCategoriesOperation.InnerError?.ErrorMessage!, AppResources.Accept_AlertButton);
                return;
             }

             var addDefaultCategoriesOperation = await _categoryPersistenceService.AddCategoriesAsync(DefaultCategories.DefaultCategoriesList);
                addDefaultCategoriesOperation.WriteLog("Add the default categories in the storage (Clear Storage)");

             if (!addDefaultCategoriesOperation.Success)
             {
                await  _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, addDefaultCategoriesOperation.InnerError?.ErrorMessage!, AppResources.Accept_AlertButton);
                return;
             }

            await _customDialogsService.ShowAlertDialogAsync
                (
                    AppResources.StorageCleared_AlertTitle, 
                    AppResources.StorageCleared_AlertContent, 
                    AppResources.Accept_AlertButton
                );
            await DirectNavigate(Routes.Dashboard);
        }
    }
}
