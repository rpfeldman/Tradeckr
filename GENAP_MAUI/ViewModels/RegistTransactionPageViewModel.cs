using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using GENAP_MAUI.InnerComponents;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class RegistTransactionPageViewModel(GlobalResources globalResources, DataRegistrationService dataRegistrationService, CategoryPersistenceService categoryPersistenceService) : BaseViewModel
    {
        private DataRegistrationService _RegistrationService = dataRegistrationService;
        private CategoryPersistenceService _CategoryPersistenceService = categoryPersistenceService;

        public GlobalResources GlobalResources { get; set; } = globalResources;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial int FixedTransactionDuration { get; set; } = 1;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial decimal Value { get; set; } = 0m;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial string PickedValue { get; set; } = "0";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial CategoryDto Category { get; set; }

        [ObservableProperty]
        public partial DateTime PickedDate { get; set; } = DateTime.Today;

        [ObservableProperty]
        public partial bool Depletion { get; set; } = true;

        [ObservableProperty]
        public partial ObservableCollection<CategoryDto> Categories { get; set; } = [];

        public bool IsIncomeSelected => !Depletion;
        public bool IsExpenseSelected => Depletion;

        [ObservableProperty]
        public partial bool IsFixed { get; set; } = false;

        partial void OnDepletionChanged(bool value)
        {
            OnPropertyChanged(nameof(IsIncomeSelected));
            OnPropertyChanged(nameof(IsExpenseSelected));
        }

        partial void OnPickedValueChanged(string value)
        {
            if (decimal.TryParse(value, out decimal newValue))
            {
                Value = newValue;
                return;
            }

            Value = 0m;
        }

        [RelayCommand(CanExecute = nameof(RegistTransactionCanExecute))]
        public async Task RegistTransaction()
        {
            var DisplayAlertTitle = "Transaccion";
            var DisplayAlertButton = "Aceptar";

            if (Depletion)
            {
                var ExpenseRegistrationTask = IsFixed ? await _RegistrationService.RegistFixedExpenseAsync(Value, DateOnly.FromDateTime(PickedDate), Category.Name, FixedTransactionDuration) : await _RegistrationService.RegistExpenseAsync(Value, DateOnly.FromDateTime(PickedDate), Category.Name);

                await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, ExpenseRegistrationTask.Success ? "Gasto registrado con exito" : ExpenseRegistrationTask.ErrorMessage, DisplayAlertButton);

                return;
            }

            var IncomeRegistrationTask = IsFixed ? await _RegistrationService.RegistFixedIncomeAsync(Value, DateOnly.FromDateTime(PickedDate), Category.Name, FixedTransactionDuration) : await _RegistrationService.RegistIncomeAsync(Value, DateOnly.FromDateTime(PickedDate), Category.Name);

            await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, IncomeRegistrationTask.Success ? "Ingreso registrado con exito" : IncomeRegistrationTask.ErrorMessage, DisplayAlertButton);

			return;
        }

        [RelayCommand]
        public async Task ReLoad() 
        {
            FixedTransactionDuration = 1;
            Value = 0m;
            PickedValue = "0";
            PickedDate = DateTime.Today;

            var getCategoryOperation = await _CategoryPersistenceService.GetCategoriesAsync();
            if (getCategoryOperation.Success)
            {
                Categories = new(getCategoryOperation.Result!);
                Category = getCategoryOperation.Result!.First();
            }
            else { await Shell.Current.DisplayAlertAsync("Error", getCategoryOperation.ErrorMessage, "Aceptar"); }
        }

        [RelayCommand] void SetIncome() => Depletion = false;
        [RelayCommand] void SetExpense() => Depletion = true;

        private bool RegistTransactionCanExecute() => Value > 0m && Value <= 1000000000m && FixedTransactionDuration >= 1;
    }
}
