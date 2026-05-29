
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GENAP_MAUI.InnerComponents;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class TransactionCategoriesPageViewModel : BaseViewModel
    {
        private GlobalResources _GR;

        public TransactionCategoriesPageViewModel(GlobalResources globalResources)
        {
            _GR = globalResources;
            Categories = new(_GR.GlobalCategories);
        }

        [ObservableProperty]
        public partial ObservableCollection<CategoryDto> Categories { get; set; }

        public Dictionary<string, string> CategoryColors = new() 
        {
            {"Rojo", "#F21313"},
            {"Verde", "#5AF213"},
            {"Azul", "#1713F2"},
            {"Amarillo", "#F2E713"},
            {"Naranja", "#F28213"},
            {"Turquesa", "#13F29D"},
            {"Celeste", "#13BAF2"},
            {"Rosa", "#E713F2"},
            {"Lima", "#C9F213"},
        };

        public List<KeyValuePair<string, string>> ColorsList => [.. CategoryColors];

        [ObservableProperty]
        public partial KeyValuePair<string, string> PickedColor { get; set; }

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
            Categories.Add(new CategoryDto(NewCategory, PickedColor.Value));

			SaveCommand.NotifyCanExecuteChanged();
            NewCategory = string.Empty;
        }

        [RelayCommand(CanExecute = nameof(SaveCanExecute))]
        public async Task Save()
        {
            _GR.GlobalCategories = new(Categories);

            NewCategory = string.Empty;

            await Shell.Current.DisplayAlertAsync("Categorias", "Se guardaron las categorias","Aceptar");
        }

        private bool AddCategoryCanExecute() => !string.IsNullOrWhiteSpace(NewCategory);
        private bool SaveCanExecute() => Categories.Count > 0;
    }
}
