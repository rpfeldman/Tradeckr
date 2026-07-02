

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using GENAP_MAUI.InnerComponents;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Transactions;
using static GENAP_MAUI.GlobalResources;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class TransactionCategoriesPageViewModel : BaseViewModel
    {
        private DataManagementService _dataManagementService;
        private CategoryPersistenceService _categoryPersistenceService;
        public TransactionCategoriesPageViewModel(CategoryPersistenceService categoryPersistenceService, DataManagementService dataManagementService)
        {
            _dataManagementService = dataManagementService;
            _categoryPersistenceService = categoryPersistenceService;

            PickedColor = GlobalResources.Colors[ColorsEnum.SteelBlue];
        }

        [ObservableProperty]
        public partial ObservableCollection<CategoryDto> Categories { get; set; } = new();

        [ObservableProperty]
        public partial ColorDto PickedColor { get; set; } 

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCategoryCommand))]
        public partial string NewCategory { get; set; } = string.Empty;

        [RelayCommand]
        public async Task DeleteCategory(CategoryDto Category)
        {
            Categories.Remove(Category);
            SaveCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(AddCategoryCanExecute))]
        public async Task AddCategory()
        {
            Categories.Add(new CategoryDto { Name = NewCategory, HexColor = PickedColor.HexColor });

			SaveCommand.NotifyCanExecuteChanged();
            NewCategory = string.Empty;
        }

        [RelayCommand(CanExecute = nameof(SaveCanExecute))]
        public async Task Save()
        {

            // I have to rethink this
         
            foreach (var category in Categories)
            {
                var existsOperation = await _categoryPersistenceService.ExistsAsync(category.Id);
                if (!existsOperation.Success)
                {
                    await Shell.Current.DisplayAlertAsync("Error", existsOperation.ErrorMessage, "Aceptar");
                }

                if (existsOperation.Result)
                {
                    var updateCategoryOperation = await _categoryPersistenceService.UpdateCategoryAsync(category.Id, category.Name, category.HexColor);
                    if (!updateCategoryOperation.Success)
                    {
                        await Shell.Current.DisplayAlertAsync("Error", updateCategoryOperation.ErrorMessage, "Aceptar");
                    }
                    continue;
                }

                var saveCategoryOperation = await _categoryPersistenceService.AddCategoryAsync(category.Name, category.HexColor);
                if (!saveCategoryOperation.Success)
                {
                    await Shell.Current.DisplayAlertAsync("Error", saveCategoryOperation.ErrorMessage, "Aceptar");
                }

                await Shell.Current.DisplayAlertAsync("Categorias", "Categorias guardadas correctamente", "Aceptar");
            }
        }

        [RelayCommand]
        public async Task ReLoad()
        {
            PickedColor = GlobalResources.Colors[ColorsEnum.SteelBlue];
            NewCategory = string.Empty;

            var getCategoryOperation = await _categoryPersistenceService.GetCategoriesAsync();
            if (getCategoryOperation.Success)
            {
                Categories = new(getCategoryOperation.Result!);
            }
            else { await Shell.Current.DisplayAlertAsync("Error", getCategoryOperation.ErrorMessage, "Aceptar"); }
        }
        private bool AddCategoryCanExecute() => !string.IsNullOrWhiteSpace(NewCategory) && !Categories.Any(c => c.Name == NewCategory) && PickedColor is not null;
        private bool SaveCanExecute() => Categories.Count > 0;
    }
}
