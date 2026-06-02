using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using GENAP_MAUI.InnerComponents;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class RegistTransactionPageViewModel(GlobalResources globalResources, DataRegistrationService dataRegistrationService) : BaseViewModel
    {
        private DataRegistrationService _RegistrationService = dataRegistrationService;

        public GlobalResources GlobalResources { get; set; } = globalResources;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial int FixedTransactionDuration { get; set; } = 1;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial decimal Value { get; set; } = 0m;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial CategoryDto Category { get; set; } = globalResources.GlobalCategories.First();

        [ObservableProperty]
        public partial DateTime PickedDate { get; set; } = DateTime.Today;

        [ObservableProperty]
        public partial bool Depletion { get; set; } = true;

        [ObservableProperty]
        public partial bool IsFixed { get; set; } = false;

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
                var ExpenseRegistrationTask = IsFixed ? await _RegistrationService.RegistExpenseAsync(Value, DateOnly.FromDateTime(PickedDate), FixedTransactionDuration, Category.CategoryName) : await _RegistrationService.RegistExpenseAsync(Value, DateOnly.FromDateTime(PickedDate), Category.CategoryName);

                await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, ExpenseRegistrationTask ? "Gasto registrado con exito" : "Ocurrio un error al registrar el gasto\nGasto no registrado", DisplayAlertButton);

                return;
            }

            var IncomeRegistrationTask = IsFixed ? await _RegistrationService.RegistIncomeAsync(Value, DateOnly.FromDateTime(PickedDate), FixedTransactionDuration, Category.CategoryName) : await _RegistrationService.RegistIncomeAsync(Value, DateOnly.FromDateTime(PickedDate), Category.CategoryName);

            await Shell.Current.DisplayAlertAsync(DisplayAlertTitle, IncomeRegistrationTask ? "Ingreso registrado con exito" : "Ocurrio un error al registrar el Ingreso\nIngreso no registrado", DisplayAlertButton);

			return;
        }


        private bool RegistTransactionCanExecute() => !string.IsNullOrWhiteSpace(Category.CategoryName) && Value > 0m && FixedTransactionDuration >= 1;
    }
}
