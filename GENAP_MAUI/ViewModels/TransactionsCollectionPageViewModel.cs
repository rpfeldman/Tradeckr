using Android.Provider;
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

        public TransactionsCollectionPageViewModel(DataProjectionService dataProjectionService)
        {
            _dataProjectionService = dataProjectionService;
        }

        [ObservableProperty]
        public partial ObservableCollection<TransactionDto> Transactions { get; set; } = [];

        [RelayCommand]
        public async Task FillTransactions()
        {
            Transactions = new(await _dataProjectionService.GetAllAsync());
        }

        [RelayCommand]
        public async Task NavigateIntoTransaction(TransactionDto transaction)
        {
            var NavProperty = new Dictionary<string, object>()
            {
                { "TransactionProperty", transaction }
            };

            await Shell.Current.GoToAsync(Routes.TransactionMenu, parameters: NavProperty);
        }
    }
}
