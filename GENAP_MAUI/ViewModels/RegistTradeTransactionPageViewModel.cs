using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class RegistTradeTransactionPageViewModel(DataRegistrationService dataRegistrationService) : BaseViewModel
    {
        private DataRegistrationService _RegistrationService = dataRegistrationService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial decimal Value { get; set; } = 0m;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial string PickedValue { get; set; } = "0";

        [ObservableProperty]
        public partial DateTime PickedDate { get; set; } = DateTime.Today;

        [ObservableProperty]
        public partial bool Depletion { get; set; } = true;
        public bool IsIncomeSelected => !Depletion;
        public bool IsExpenseSelected => Depletion;

        partial void OnDepletionChanged(bool value)
        {
            OnPropertyChanged(nameof(IsIncomeSelected));
            OnPropertyChanged(nameof(IsExpenseSelected));
        }

        partial void OnPickedValueChanged(string value)
        {
            if(decimal.TryParse(value, out decimal newValue))
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
                var ExpenseRegistrationTask = await _RegistrationService.RegistExpenseAsync(Value, DateOnly.FromDateTime(PickedDate), "Trading");

                await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, ExpenseRegistrationTask ? "Gasto registrado con exito" : "Ocurrio un error al registrar el gasto\nGasto no registrado", DisplayAlertButton);

                return;
            }

            var IncomeRegistrationTask = await _RegistrationService.RegistIncomeAsync(Value, DateOnly.FromDateTime(PickedDate), "Trading");

            await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, IncomeRegistrationTask ? "Ingreso registrado con exito" : "Ocurrio un error al registrar el Ingreso\nIngreso no registrado", DisplayAlertButton);

            return;
        }

        [RelayCommand]
        public void ReLoad()
        {
            Value = 0m;
            PickedValue = "0";
            PickedDate = DateTime.Today;
            Depletion = true;
        }

        [RelayCommand] void SetIncome() => Depletion = false;
        [RelayCommand] void SetExpense() => Depletion = true;
        private bool RegistTransactionCanExecute() => Value > 0m && Value <= 1000000000m;
    }
}
