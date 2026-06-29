using CommunityToolkit.Mvvm.ComponentModel;
using DomainModel;
using DataServices;
using GENAP_MAUI.InnerComponents;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace GENAP_MAUI.ViewModels
{
    [QueryProperty(nameof(TransactionId), "TransactionProperty")]
    public sealed partial class TransactionPageViewModel : BaseViewModel
    {
        private DataProjectionService _dataProjectionService;
        private DataManagementService _dataManagementService;

        public GlobalResources GlobalResources { get; }
        public TransactionPageViewModel(DataProjectionService dataProjectionService, DataManagementService dataManagementService, GlobalResources globalResources)
        {
            _dataProjectionService = dataProjectionService;
            _dataManagementService = dataManagementService;
            GlobalResources = globalResources;

            PickedCategory = GlobalResources.GlobalCategories.First();
        }

        [ObservableProperty]
        public partial int TransactionId { get; set; } 

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteFixedTransactionCommand))]
        public partial TransactionDto Transaction { get; set; } = new();

        [ObservableProperty]
        public partial DateTime PickedDate { get; set; } 

        [ObservableProperty]
        public partial CategoryDto PickedCategory { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(UpdateTransactionCommand))]
        public partial decimal PickedValue { get; set; }

        async partial void OnTransactionIdChanged(int value)
        {
            var getTransactionOperation = await _dataProjectionService.GetTransactionAsync(value);

            getTransactionOperation.Match
            (
                some: (t) => Transaction = t,
                none: async () => { await Shell.Current.DisplayAlertAsync("Error", "Transaccion inexistente", "Aceptar"); await DirectNavigate(Routes.TransactionsList); }
            ); 

            PickedCategory = GlobalResources.GlobalCategories.Where(c => c.CategoryName == Transaction.Category).Count() > 0 ? GlobalResources.GlobalCategories.Where(c => c.CategoryName == Transaction.Category).First() : new CategoryDto(Transaction.Category, GlobalResources.Colors.Values.First(), default);
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
            var updateTransactionOperation = await _dataManagementService.UpdateTransactionAsync(TransactionId, PickedValue, DateOnly.FromDateTime(PickedDate), PickedCategory.CategoryName, Transaction.Depletion);

            await Shell.Current.DisplayAlertAsync("Editar", updateTransactionOperation.Success ? "Se ha guardado el movimiento correctamente" : updateTransactionOperation.ErrorMessage, "Aceptar");
        }

        private bool DeleteFixedTransactionCanExecute => Transaction is FixedTransactionDto;

        private bool UpdateTransactionCanExecute => PickedValue > 0; 
    }
}
