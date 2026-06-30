

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

        public GlobalResources GlobalResources { get; }

        public TransactionCategoriesPageViewModel(GlobalResources globalResources, DataManagementService dataManagementService)
        {
            _dataManagementService = dataManagementService;
            GlobalResources = globalResources;

            Categories = new(GlobalResources.GlobalCategories.Select(c => new CategoryDto(c.CategoryName, c.Color, c.CategoryId)));
            PickedColor = GlobalResources.Colors[ColorsEnum.SteelBlue];
        }

        [ObservableProperty]
        public partial ObservableCollection<CategoryDto> Categories { get; set; }

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
            Categories.Add(new CategoryDto(NewCategory, PickedColor, GlobalResources.GlobalCategories.Count));

			SaveCommand.NotifyCanExecuteChanged();
            NewCategory = string.Empty;
        }

        [RelayCommand(CanExecute = nameof(SaveCanExecute))]
        public async Task Save()
        {
            foreach (var item in Categories)
            {
                CategoryDto? oldCategory = GlobalResources.GlobalCategories.Where(c => c.CategoryId == item.CategoryId).FirstOrDefault();

                if (oldCategory is not null && oldCategory.CategoryName != item.CategoryName)
                {
                    var OldName = oldCategory.CategoryName;
                    var NewName = item.CategoryName;

                    var renameCategoryOperation = await _dataManagementService.RenameCategoryAsync(OldName, NewName);

                    /*
                    if (!renameCategoryOperation.Success)
                    {
                        await Shell.Current.DisplayAlertAsync("Error", renameCategoryOperation.ErrorMessage, "Aceptar");
                    }
                    */
                }
            }

            GlobalResources.GlobalCategories.Clear();

            foreach (var item in Categories)
            {
                GlobalResources.GlobalCategories.Add(item);
            }

            NewCategory = string.Empty;

            await Shell.Current.DisplayAlertAsync("Categorias", "Se guardaron las categorias", "Aceptar");
        }

        [RelayCommand]
        public void ReLoad()
        {
            Categories = new(GlobalResources.GlobalCategories.Select(c => new CategoryDto(c.CategoryName, c.Color, c.CategoryId)));
            PickedColor = GlobalResources.Colors[ColorsEnum.SteelBlue];
            NewCategory = string.Empty;
        }
        private bool AddCategoryCanExecute() => !string.IsNullOrWhiteSpace(NewCategory) && Categories.Where(c => c.CategoryName == NewCategory).Count() == 0;
        private bool SaveCanExecute() => Categories.Count > 0;
    }
}
