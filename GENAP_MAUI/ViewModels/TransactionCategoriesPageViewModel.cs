
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
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        public partial ObservableCollection<CategoryDto> Categories { get; set; } = new();

        private List<CategoryDto> DeletedCategories { get; set; } = [];

        private List<CategoryDto> AddedCategories { get; set; } = [];

        private CategoryDto[] OldCategories { get; set; } = [];

        [ObservableProperty]
        public partial ColorDto PickedColor { get; set; } 

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCategoryCommand))]
        public partial string NewCategory { get; set; } = string.Empty;

        [RelayCommand]
        public async Task DeleteCategory(CategoryDto Category)
        {
            Categories.Remove(Category);

            if (OldCategories.Any(c => c.Id == Category.Id))
            {
                DeletedCategories.Add(Category);
            }
            else { AddedCategories.Remove(Category); }
            
            SaveCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(AddCategoryCanExecute))]
        public async Task AddCategory()
        {
            var newCategory = new CategoryDto { Name = NewCategory, HexColor = PickedColor.HexColor };

            Categories.Add(newCategory);
            AddedCategories.Add(newCategory);

			SaveCommand.NotifyCanExecuteChanged();
            NewCategory = string.Empty;
        }

        [RelayCommand(CanExecute = nameof(SaveCanExecute))]
        public async Task Save()
        {
            // I definitely have to review this....

            const string ActionTitle = "Se ha detectado que has borrado una o mas categorias\n¿que desea hacer con los movimiento asociados a las mismas?";
            const string ActionCancelBtn = "Cancelar";
            const string ActionDeleteAllBtn = "Eliminar todos los movimientos asociados";
            const string ActionPreserveBtn = "Mantener los movimientos sin categoria";

            Task<OperationResult> ActionMethod = Task.FromResult(OperationResult.SuccessfulOperation());
            if (DeletedCategories.Count != 0)
            {
                var action = await Shell.Current.DisplayActionSheetAsync(ActionTitle, ActionCancelBtn, ActionDeleteAllBtn, buttons: [ActionPreserveBtn]);

                switch (action)
                {
                    case ActionCancelBtn: await ReLoad(); return;
                    case ActionDeleteAllBtn: ActionMethod = _dataManagementService.RemoveFromCategories([.. DeletedCategories.Select(c => c.Name)]); break;
                    case ActionPreserveBtn: break;
                }   
            }

            List<CategoryDto> updatedCategories = [];
            foreach (var category in Categories.Join(OldCategories, a => a.Id, b => b.Id, (a, b) => new { A = a, B = b }).Where(cat => cat.A.Name != cat.B.Name))
            {
                updatedCategories.Add(category.A);

                var renameCategoryOperation = await _dataManagementService.RenameCategoryAsync(category.B.Name, category.A.Name);

                if (!renameCategoryOperation.Success)
                {
                    await Shell.Current.DisplayAlertAsync("Error", renameCategoryOperation.ErrorMessage, "Aceptar");
                    return;
                }
            }

            var Operations = await Task.WhenAll(
               _categoryPersistenceService.RemoveCategoriesAsync([.. DeletedCategories]),
               _categoryPersistenceService.AddCategoriesAsync([.. AddedCategories]),
               _categoryPersistenceService.UpdateCategoriesAsync([.. updatedCategories]),
               ActionMethod
               );

            if (Operations.Any(o => !o.Success))
            {
                await Shell.Current.DisplayAlertAsync("Error", Operations.Where(o => !o.Success).First().ErrorMessage, "Aceptar");
                await ReLoad();
                return;
            }

            await Shell.Current.DisplayAlertAsync("Categorias", "Se guardaron las categorias correctamente", "Aceptar");
            await ReLoad();
        }

        [RelayCommand]
        public async Task ReLoad()
        {
            PickedColor = GlobalResources.Colors[ColorsEnum.SteelBlue];
            NewCategory = string.Empty;
            DeletedCategories.Clear();
            AddedCategories.Clear();

            var getCategoryOperation = await _categoryPersistenceService.GetCategoriesAsync();
            if (getCategoryOperation.Success)
            {
                Categories = new(getCategoryOperation.Result!.Select(c => new CategoryDto() { Name = c.Name, HexColor = c.HexColor, Id = c.Id }));
                OldCategories = [.. getCategoryOperation.Result!];
            }
            else { await Shell.Current.DisplayAlertAsync("Error", getCategoryOperation.ErrorMessage, "Aceptar"); }
        }
        private bool AddCategoryCanExecute() => !string.IsNullOrWhiteSpace(NewCategory) && !Categories.Any(c => c.Name == NewCategory) && PickedColor is not null;
        private bool SaveCanExecute() => Categories.Count > 0 && !Categories.Any(c => string.IsNullOrWhiteSpace(c.Name));
    }
}
