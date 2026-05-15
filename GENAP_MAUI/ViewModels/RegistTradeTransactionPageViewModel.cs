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
        public partial DateTime PickedDate { get; set; } = DateTime.Today;

        [ObservableProperty]
        public partial bool Depletion { get; set; } = true;

        [RelayCommand]
        public void ChangeDepletion()
        {
            Depletion = Depletion ? false : true;
        }

        [RelayCommand(CanExecute = nameof(RegistTransactionCanExecute))]
        public async Task RegistTransaction()
        {
            var DisplayAlertTitle = "Transaccion";
            var DisplayAlertButton = "Aceptar";

            if (Depletion)
            {
                var ExpenseRegistrationTask = await _RegistrationService.RegistExpense(Value, DateOnly.FromDateTime(PickedDate), "Trading");

                await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, ExpenseRegistrationTask ? "Gasto registrado con exito" : "Ocurrio un error al registrar el gasto\nGasto no registrado", DisplayAlertButton);

                return;
            }

            var IncomeRegistrationTask = await _RegistrationService.RegistIncome(Value, DateOnly.FromDateTime(PickedDate), "Trading");

            await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, IncomeRegistrationTask ? "Ingreso registrado con exito" : "Ocurrio un error al registrar el Ingreso\nIngreso no registrado", DisplayAlertButton);

            return;
        }

        private bool RegistTransactionCanExecute() => Value > 0m;
    }
}
