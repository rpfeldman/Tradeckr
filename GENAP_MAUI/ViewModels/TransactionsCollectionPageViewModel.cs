
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

            PickedTimePeriod = GlobalResources.TimePeriods.Where(d => d.Key == GlobalResources.TimePeriodsEnum.Month).First();
        }

        [ObservableProperty]
        public partial ObservableCollection<TransactionDto> Transactions { get; set; } = [];

        [ObservableProperty]
        public partial KeyValuePair<GlobalResources.TimePeriodsEnum, string> PickedTimePeriod { get; set; }

        async partial void OnPickedTimePeriodChanged(KeyValuePair<GlobalResources.TimePeriodsEnum, string> value)
        {
            try
            {
                await ReloadTransactions(value.Key);
            }
            catch (Exception x)
            {
                // FUTURE: Exception managment

                Console.WriteLine(x);
            }
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
        public async Task ReloadTransactions(GlobalResources.TimePeriodsEnum timePeriod)
        {
            Task<OperationResult<List<TransactionDto>>>? getTransactionsTask;
            var today = DateOnly.FromDateTime(DateTime.Today);

            switch (timePeriod)
            {
                case GlobalResources.TimePeriodsEnum.Historical:
                    getTransactionsTask = _dataProjectionService.GetAllAsync(order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.HistoricalToday:
                    getTransactionsTask = _dataProjectionService.GetAllByPredicateAsync(t => t.Date <= today, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.Month:
                    getTransactionsTask = _dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, order:DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.ThirtyDays:
                    getTransactionsTask = _dataProjectionService.GetAllByPredicateAsync(t => t.Date.DayOfYear >= (today.DayOfYear - 30) && t.Date <= today && t.Date.Year == today.Year, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.ThreeMonths:
                    getTransactionsTask = _dataProjectionService.GetAllByPredicateAsync(t => t.Date.Month >= (today.Month - 3) && t.Date.Month <= today.Month && t.Date.Year == today.Year, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.Semester:
                    int MinBound;
                    int MaxBound;

                    if (today.Month > 6)
                    { MinBound = 7; MaxBound = 12; }
                    else { MinBound = 1; MaxBound = 6; }

                    getTransactionsTask = _dataProjectionService.GetAllByPredicateAsync(t => t.Date.Month >= MinBound && t.Date.Month <= MaxBound && t.Date.Year == today.Year, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.Year:
                    getTransactionsTask = _dataProjectionService.GetAllByYearAsync(today.Year, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.Today:
                    getTransactionsTask = _dataProjectionService.GetAllByDateAsync(today, order: DataProjectionService.Order.OrderByDate);
                    break;

                default:
                    getTransactionsTask = _dataProjectionService.GetAllAsync(order: DataProjectionService.Order.OrderByDate);
                    break;
            }

            var GetTransactionsOperation = await getTransactionsTask;

            if (GetTransactionsOperation.Success)
            {
                Transactions = new(GetTransactionsOperation.Result!);
            }else { /* to - do */ }
        }

    }
}
