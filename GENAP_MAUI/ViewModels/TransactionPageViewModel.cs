using CommunityToolkit.Mvvm.ComponentModel;
using DomainModel;
using DataServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace GENAP_MAUI.ViewModels
{
    [QueryProperty(nameof(Transaction), "TransactionProperty")]
    public sealed partial class TransactionPageViewModel : BaseViewModel
    {
        private DataProjectionService _dataProjectionService;
        private DataManagementService _dataManagementService;
        public TransactionPageViewModel(DataProjectionService dataProjectionService, DataManagementService dataManagementService)
        {
            _dataProjectionService = dataProjectionService;
            _dataManagementService = dataManagementService;
        }

        [ObservableProperty]
        public partial int TransactionId { get; set; } 

        [ObservableProperty]
        public partial TransactionDto Transaction { get; set; } = new();

        async partial void OnTransactionIdChanged(int value)
        {
            
        }
    }
}
