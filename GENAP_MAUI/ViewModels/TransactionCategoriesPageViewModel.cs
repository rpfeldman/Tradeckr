

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
                if (GlobalResources.GlobalCategories.Where(c => c.CategoryId == item.CategoryId).Count() == 1 && GlobalResources.GlobalCategories.Where(c => c.CategoryId == item.CategoryId).First().CategoryName != item.CategoryName)
                {
                    var OldName = GlobalResources.GlobalCategories.Where(c => c.CategoryId == item.CategoryId).First().CategoryName;
                    var NewName = item.CategoryName;

                    await UpdateTransactionsCategories(OldName, NewName);
                }
            }

            GlobalResources.GlobalCategories.Clear();

            foreach (var item in Categories)
            {
                GlobalResources.GlobalCategories.Add(item);
            }

            NewCategory = string.Empty;

            await Shell.Current.DisplayAlertAsync("Categorias", "Se guardaron las categorias","Aceptar");
        }

        [RelayCommand]
        public void ReLoad()
        {
            Categories = new(GlobalResources.GlobalCategories.Select(c => new CategoryDto(c.CategoryName, c.Color, c.CategoryId)));
            PickedColor = GlobalResources.Colors[ColorsEnum.SteelBlue];
            NewCategory = string.Empty;
        }

        private async Task UpdateTransactionsCategories(string OldName, string NewName)
        {
            await _dataManagementService.RenameCategoryAsync(OldName, NewName);
        }

        private bool AddCategoryCanExecute() => !string.IsNullOrWhiteSpace(NewCategory) && Categories.Where(c => c.CategoryName == NewCategory).Count() == 0;
        private bool SaveCanExecute() => Categories.Count > 0;
    }
}
