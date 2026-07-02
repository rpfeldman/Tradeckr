using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using GENAP_MAUI.InnerComponents;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    [QueryProperty(nameof(TransactionId), "TransactionProperty")]
    public sealed partial class TransactionPageViewModel : BaseViewModel
    {
        private DataProjectionService _dataProjectionService;
        private DataManagementService _dataManagementService;
        private CategoryPersistenceService _categoryPersistenceService;
        public TransactionPageViewModel(DataProjectionService dataProjectionService, DataManagementService dataManagementService, CategoryPersistenceService categoryPersistenceService)
        {
            _dataProjectionService = dataProjectionService;
            _dataManagementService = dataManagementService;
            _categoryPersistenceService = categoryPersistenceService;
        }

        [ObservableProperty]
        public partial int TransactionId { get; set; } 

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteFixedTransactionCommand))]
        public partial TransactionDto Transaction { get; set; } = new();

        [ObservableProperty]
        public partial DateTime PickedDate { get; set; }

        [ObservableProperty]
        public partial CategoryDto PickedCategory { get; set; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UpdateTransactionCommand))]
        public partial decimal PickedValue { get; set; }


        [ObservableProperty]
        public partial ObservableCollection<CategoryDto> Categories { get; set; } = new();

        async partial void OnTransactionIdChanged(int value)
        {
            var getTransactionOperation = await _dataProjectionService.GetTransactionAsync(value);

            getTransactionOperation.Match
            (
                some: (t) => Transaction = t,
                none: async () => { await Shell.Current.DisplayAlertAsync("Error", "Transaccion inexistente", "Aceptar"); await DirectNavigate(Routes.TransactionsList); }
            );

            var getCategoriesOperation = await _categoryPersistenceService.GetCategoriesAsync();

            if (!getCategoriesOperation.Success)
            {
                await Shell.Current.DisplayAlertAsync("Error", getCategoriesOperation.ErrorMessage, "Aceptar");
                return;
            }

            Categories = new(getCategoriesOperation.Result!);
            PickedCategory = getCategoriesOperation.Result!.Where(c => c.Name == Transaction.Category).First();
            PickedDate = Transaction.Date.ToDateTime(TimeOnly.MinValue);
            PickedValue = Transaction.Value;
        }

        [RelayCommand]
        public async Task DeleteTransaction()
        {
            var deleteTransactionOperation = await _dataManagementService.RemoveTransactionAsync(TransactionId);

            await Shell.Current.DisplayAlertAsync("Eliminar", deleteTransactionOperation.Success ? "Movimiento eliminado correctamente" : deleteTransactionOperation.ErrorMessage, "Aceptar");

            await DirectNavigate(Routes.TransactionsList);
        }

        [RelayCommand(CanExecute = nameof(DeleteFixedTransactionCanExecute))]
        public async Task DeleteFixedTransaction(bool FromToday)
        {
            var transaction = Transaction as FixedTransactionDto;
            var deleteCollectionOperation = FromToday ? await _dataManagementService.RemoveFixedTransactionAsync(transaction!.FixedTransactionId, transaction.Duration) : await _dataManagementService.RemoveFixedTransactionAsync(transaction!.FixedTransactionId);

            await Shell.Current.DisplayAlertAsync("Eliminar", deleteCollectionOperation.Success ? "Movimientos eliminado correctamente" : deleteCollectionOperation.ErrorMessage, "Aceptar");

            await DirectNavigate(Routes.TransactionsList);
        }

        [RelayCommand(CanExecute = nameof(UpdateTransactionCanExecute))]
        public async Task UpdateTransaction()
        {
            var updateTransactionOperation = await _dataManagementService.UpdateTransactionAsync(TransactionId, PickedValue, DateOnly.FromDateTime(PickedDate), PickedCategory.Name, Transaction.Depletion);

            await Shell.Current.DisplayAlertAsync("Editar", updateTransactionOperation.Success ? "Se ha guardado el movimiento correctamente" : updateTransactionOperation.ErrorMessage, "Aceptar");
        }

        private bool DeleteFixedTransactionCanExecute => Transaction is FixedTransactionDto;

        private bool UpdateTransactionCanExecute => PickedValue > 0; 
    }
}
