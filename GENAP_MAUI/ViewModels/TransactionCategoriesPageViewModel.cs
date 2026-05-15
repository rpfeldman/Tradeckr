
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class TransactionCategoriesPageViewModel : BaseViewModel
    {
        private GlobalResources _GR;

        [ObservableProperty]
        public partial ObservableCollection<string> Categories { get; set; }

        public TransactionCategoriesPageViewModel(GlobalResources globalResources)
        {
            _GR = globalResources;
            Categories = _GR.GlobalCategories;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCategoryCommand))]
        public partial string NewCategory { get; set; } = string.Empty;

        [RelayCommand]
        public async Task DeleteCategory(string Category)
        {
            Categories.Remove(Category);
        }

        [RelayCommand(CanExecute = nameof(AddCategoryCanExecute))]
        public async Task AddCategory()
        {
            Categories.Add(NewCategory);
        }

        [RelayCommand]
        public async Task Save()
        {
            _GR.GlobalCategories = Categories;
            await Shell.Current.DisplayAlertAsync("Categorias", "Se guardaron las categorias","Aceptar");
        }

        private bool AddCategoryCanExecute() => !string.IsNullOrWhiteSpace(NewCategory);
    }
}
