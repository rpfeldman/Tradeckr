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
    public sealed partial class RegistTransactionPageViewModel(DataRegistrationService dataRegistrationService, CategoryPersistenceService categoryPersistenceService) : BaseViewModel
    {
        private DataRegistrationService _RegistrationService = dataRegistrationService;
        private CategoryPersistenceService _CategoryPersistenceService = categoryPersistenceService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial int FixedTransactionDuration { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial decimal Value { get; set; } 

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial string PickedValue { get; set; } 

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial CategoryDto Category { get; set; }

        [ObservableProperty]
        public partial DateTime PickedDate { get; set; }

        [ObservableProperty]
        public partial CurrencyDto PickedCurrency { get; set; }

        [ObservableProperty]
        public partial bool Depletion { get; set; }

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

            Value = CurrencyConverterService.CurrencyToTfu(Value, PickedCurrency); 

            if (Depletion)
            {
                var ExpenseRegistrationOperation = IsFixed ? await _RegistrationService.RegistFixedExpenseAsync(Value, DateOnly.FromDateTime(PickedDate), Category.Name, FixedTransactionDuration) : await _RegistrationService.RegistExpenseAsync(Value, DateOnly.FromDateTime(PickedDate), Category.Name);

                ExpenseRegistrationOperation.WriteLog
                    (
                        IsFixed ? 

                            $"Save new series of expenses with the following attributes: category: '{Category.Name}'. Value: '{Value:N2} {PickedCurrency.IsoCode}$'. Date: '{PickedDate:dd/MM/yyyy}'. Duration: '{FixedTransactionDuration}'" :
                             
                            $"Save new expense with the following attributes: category: '{Category.Name}'. Value: '{Value:N2} {PickedCurrency.IsoCode}$'. Date: '{PickedDate:dd/MM/yyyy}'"
                    
                    );


                await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, ExpenseRegistrationOperation.Success ? "Gasto registrado con exito" : ExpenseRegistrationOperation.InnerError?.ErrorMessage, DisplayAlertButton);

                return;
            }

            var IncomeRegistrationOperation = IsFixed ? await _RegistrationService.RegistFixedIncomeAsync(Value, DateOnly.FromDateTime(PickedDate), Category.Name, FixedTransactionDuration) : await _RegistrationService.RegistIncomeAsync(Value, DateOnly.FromDateTime(PickedDate), Category.Name);

            IncomeRegistrationOperation.WriteLog
                (
                    IsFixed ? 

                    $"Save new series of income with the following attributes: category: '{Category.Name}'. Value: '{Value:N2} {PickedCurrency.IsoCode}$'. Date: '{PickedDate:dd/MM/yyyy}'. Duration: '{FixedTransactionDuration}'" :
                             
                    $"Save new income with the following attributes: category: '{Category.Name}'. Value: '{Value:N2} {PickedCurrency.IsoCode}$'. Date: '{PickedDate:dd/MM/yyyy}'"
                );

            await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, IncomeRegistrationOperation.Success ? "Ingreso registrado con exito" : IncomeRegistrationOperation.InnerError?.ErrorMessage, DisplayAlertButton);

			return;
        }

        [RelayCommand]
        public async Task ReLoad() 
        {
            FixedTransactionDuration = 1;
            Value = 0m;
            PickedValue = string.Empty;
            PickedDate = DateTime.Today;
            PickedCurrency = GlobalResources.DefaultCommonCurrency;

            var getCategoriesOperation = await _CategoryPersistenceService.GetCategoriesAsync();
            if (getCategoriesOperation.Success)
            {
                Categories = new(getCategoriesOperation.Result!);
                Category = getCategoriesOperation.Result!.First();
            }
            else { await Shell.Current.DisplayAlertAsync("Error", getCategoriesOperation.InnerError?.ErrorMessage, "Aceptar"); }
        }

        [RelayCommand] void SetIncome() => Depletion = false;
        [RelayCommand] void SetExpense() => Depletion = true;

        private bool RegistTransactionCanExecute() => Value > 0m && Value <= 1000000000m && FixedTransactionDuration >= 1;
    }
}
