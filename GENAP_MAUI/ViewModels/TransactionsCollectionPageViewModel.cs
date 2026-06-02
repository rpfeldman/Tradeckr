
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class TransactionsCollectionPageViewModel : BaseViewModel
    {
        private DataProjectionService _dataProjectionService;
        private DataManagementService _dataManagementService;

        public TransactionsCollectionPageViewModel(DataProjectionService dataProjectionService, DataManagementService dataManagementService)
        {
            _dataProjectionService = dataProjectionService;
            _dataManagementService = dataManagementService;
        }

        [ObservableProperty]
        public partial ObservableCollection<TransactionDto> Transactions { get; set; } = [];

        [RelayCommand]
        public async Task FillTransactions()
        {
            Transactions = new(await _dataProjectionService.GetAllAsync());
        }

        [RelayCommand]
        public async Task NavigateIntoTransaction(int TransactionId)
        {
            var NavProperty = new Dictionary<string, object>()
            {
                { "TransactionProperty", TransactionId }
            };

            await Shell.Current.GoToAsync(Routes.TransactionMenu, parameters: NavProperty);
        }

        [RelayCommand]
        public async Task RestartData()
        {
            var RestartDataSuccess = await _dataManagementService.RestartData();

            await Shell.Current.DisplayAlertAsync("Eliminar", RestartDataSuccess ? "Se han reiniciado los datos" : "No se ha podido reiniciar los datos", "Aceptar");

            await Navigate(Routes.Dashboard);
        }

    }
}
