
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using GENAP_MAUI.CustomViews;
using GENAP_MAUI.InnerComponents;
using Serilog;
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
        private readonly DataManagementService _dataManagementService;
        private readonly CategoryPersistenceService _categoryPersistenceService;
        private readonly CustomDialogsService _customDialogsService;
        public TransactionCategoriesPageViewModel(CategoryPersistenceService categoryPersistenceService, DataManagementService dataManagementService, CustomDialogsService customDialogsService)
        {
            _dataManagementService = dataManagementService;
            _categoryPersistenceService = categoryPersistenceService;
            _customDialogsService = customDialogsService;

            PickedColor = GlobalResources.Colors[ColorsEnum.SteelBlue];
        }

        private bool _IsDeciding = false;

        private List<CategoryDto> DeletedCategories { get; set; } = [];

        private List<CategoryDto> AddedCategories { get; set; } = [];

        private CategoryDto[] OldCategories { get; set; } = [];

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
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

            if (OldCategories.Any(c => c.Id == Category.Id))
            {
                DeletedCategories.Add(Category);
            }
            else { AddedCategories.Remove(Category); }

            Log.Information($"Removed category {Category.Name}");
            
            SaveCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(AddCategoryCanExecute))]
        public async Task AddCategory()
        {
            var newCategory = new CategoryDto { Name = NewCategory, HexColor = PickedColor.HexColor };

            Categories.Add(newCategory);
            AddedCategories.Add(newCategory);

             Log.Information($"Added category '{NewCategory}' '{PickedColor.DisplayName}' (Categories page)");

			SaveCommand.NotifyCanExecuteChanged();
            NewCategory = string.Empty;   
        }

        [RelayCommand(CanExecute = nameof(SaveCanExecute))]
        public async Task Save()
        {
            // I definitely have to review this....

            const string ActionTitle = "Se ha detectado que has borrado una o mas categorias";
            const string ActionContent = "¿que desea hacer con los movimiento asociados a las mismas?";
            const string ActionDeleteAllBtn = "Eliminar todos los movimientos asociados";
            const string ActionPreserveBtn = "Mantener los movimientos sin categoria";

            Task<OperationResult> ActionMethod = Task.FromResult(OperationResult.SuccessfulOperation());
            if (DeletedCategories.Count != 0)
            {
                _IsDeciding = true;
                    var action = await _customDialogsService.ShowDoubleActionDialogAsync(ActionTitle, ActionContent, ActionDeleteAllBtn, ActionPreserveBtn); // this is reinstancing the ViewModel. to-fix. 
                _IsDeciding = false;

                switch (action)
                {
                    case 0: await ReLoad(); return;
                    case 1: ActionMethod = _dataManagementService.RemoveFromCategories([.. DeletedCategories.Select(c => c.Name)]); break;
                    case 2: break;
                }   
            }

            if (Categories.DistinctBy(c => c.Name).Count() != Categories.Count)
            {
                await _customDialogsService.ShowAlertDialogAsync("Error", "No puede haber 2 categorias con el mismo nombre\n\nNo se guardaron las categorias", "Aceptar");
                return;
            }

            List<CategoryDto> updatedCategories = [];

            var updatedCategoriesList = Categories.Join(OldCategories, a => a.Id, b => b.Id, (a, b) => new { A = a, B = b }).Where(cat => cat.A.Name != cat.B.Name);
            foreach (var category in updatedCategoriesList)
            {
                updatedCategories.Add(category.A);

                var renameCategoryOperation = await _dataManagementService.RenameCategoryAsync(category.B.Name, category.A.Name);
                    renameCategoryOperation.WriteLog($"Rename movements with the category '{category.B.Name}' to '{category.A.Name}' (Categories page)");

                if (!renameCategoryOperation.Success)
                {
                   await _customDialogsService.ShowAlertDialogAsync("Error", renameCategoryOperation.InnerError?.ErrorMessage!, "Aceptar");
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
                await _customDialogsService.ShowAlertDialogAsync("Error", Operations.Where(o => !o.Success).First().InnerError?.ErrorMessage!, "Aceptar");
                await ReLoad();
                return;
            }

            await _customDialogsService.ShowAlertDialogAsync("Categorias guardadas", "Se guardaron las categorias correctamente", "Aceptar");
                Log.Information("Categories updated and saved in the storage (Categories page)");

            await ReLoad();
        }

        [RelayCommand]
        public async Task ReLoad()
        {
            if (_IsDeciding) { return; }

            PickedColor = GlobalResources.Colors[ColorsEnum.SteelBlue];
            NewCategory = string.Empty;
            DeletedCategories.Clear();
            AddedCategories.Clear();

            var getCategoryOperation = await _categoryPersistenceService.GetCategoriesAsync();
                getCategoryOperation.WriteLog("Bring categories from the storage (Categories page)");

            if (getCategoryOperation.Success)
            {
                Categories = new(getCategoryOperation.Result!.Select(c => new CategoryDto() { Name = c.Name, HexColor = c.HexColor, Id = c.Id }));
                OldCategories = [.. getCategoryOperation.Result!];
            }
            else { await _customDialogsService.ShowAlertDialogAsync("Error", getCategoryOperation.InnerError?.ErrorMessage!, "Aceptar"); }
        }
        private bool AddCategoryCanExecute() => !string.IsNullOrWhiteSpace(NewCategory) && !Categories.Any(c => c.Name == NewCategory) && PickedColor is not null && NewCategory.Length <= BoundsConst.CategoryNameLimit;
        private bool SaveCanExecute() => Categories.Count > 0 && !Categories.Any(c => string.IsNullOrWhiteSpace(c.Name));
    }
}
