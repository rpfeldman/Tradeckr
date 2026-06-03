using CommunityToolkit.Mvvm.ComponentModel;
using DomainModel;
using DataServices;
using GENAP_MAUI.InnerComponents;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
        public partial CategoryDto PickedCategory { get; set; }

        async partial void OnTransactionIdChanged(int value)
        {
            Transaction = await _dataProjectionService.GetTransactionAsync(value) ?? Transaction;

            PickedCategory = GlobalResources.GlobalCategories.Where(c => c.CategoryName == Transaction.Category).Count() > 0 ? GlobalResources.GlobalCategories.Where(c => c.CategoryName == Transaction.Category).First() : new CategoryDto(Transaction.Category, "", default);
        }

        [RelayCommand]
        public async Task DeleteTransaction()
        {
            var DeleteTransactionSuccess = await _dataManagementService.RemoveTransactionAsync(TransactionId);

            await Shell.Current.DisplayAlertAsync("Eliminar", DeleteTransactionSuccess ? "Movimiento eliminado correctamente" : "No se ha podido eliminar el movimiento", "Aceptar");

            await Navigate(Routes.TransactionsList);
        }

        [RelayCommand(CanExecute = nameof(DeleteFixedTransactionCanExecute))]
        public async Task DeleteFixedTransaction(bool FromToday)
        {
            var transaction = Transaction as FixedTransactionDto;
            var DeleteCollectionSuccess = FromToday ? await _dataManagementService.RemoveFixedTransaction(transaction!.FixedTransactionId, transaction.Duration) : await _dataManagementService.RemoveFixedTransaction(transaction!.FixedTransactionId);

            await Shell.Current.DisplayAlertAsync("Eliminar", DeleteCollectionSuccess ? "Movimientos eliminado correctamente" : "No se ha podido eliminar los movimientos", "Aceptar");

            await Navigate(Routes.TransactionsList);
        }

        private bool DeleteFixedTransactionCanExecute => Transaction is FixedTransactionDto;
    }
}
