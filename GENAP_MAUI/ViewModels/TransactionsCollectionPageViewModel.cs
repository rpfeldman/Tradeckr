
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

        public GlobalResources GlobalResources { get; }

        public TransactionsCollectionPageViewModel(DataProjectionService dataProjectionService, DataManagementService dataManagementService, GlobalResources globalResources)
        {
            _dataProjectionService = dataProjectionService;
            _dataManagementService = dataManagementService;

            GlobalResources = globalResources;

            PickedTimePeriod = GlobalResources.TimePeriods.First();
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

        [RelayCommand]
        public async Task RestartData()
        {
            var RestartDataSuccess = await _dataManagementService.RestartDataAsync();

            await Shell.Current.DisplayAlertAsync("Eliminar", RestartDataSuccess ? "Se han reiniciado los datos" : "No se ha podido reiniciar los datos", "Aceptar");

            await Navigate(Routes.Dashboard);
        }

        public async Task ReloadTransactions(GlobalResources.TimePeriodsEnum timePeriod)
        {
            Task<List<TransactionDto>>? GetTransactionsTask;
            var today = DateOnly.FromDateTime(DateTime.Today);

            switch (timePeriod)
            {
                case GlobalResources.TimePeriodsEnum.Historical:
                    GetTransactionsTask = _dataProjectionService.GetAllAsync(order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.HistoricalToday:
                    GetTransactionsTask = _dataProjectionService.GetAllByPredicateAsync(t => t.Date <= today, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.Month:
                    GetTransactionsTask = _dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, order:DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.ThirtyDays:
                    GetTransactionsTask = _dataProjectionService.GetAllByPredicateAsync(t => t.Date.DayOfYear >= (today.DayOfYear - 30) && t.Date <= today && t.Date.Year == today.Year, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.ThreeMonths:
                    GetTransactionsTask = _dataProjectionService.GetAllByPredicateAsync(t => t.Date.Month >= (today.Month - 3) && t.Date.Month <= today.Month && t.Date.Year == today.Year, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.Semester:
                    int MinBound;
                    int MaxBound;

                    if (today.Month > 6)
                    { MinBound = 7; MaxBound = 12; }
                    else { MinBound = 1; MaxBound = 6; }

                    GetTransactionsTask = _dataProjectionService.GetAllByPredicateAsync(t => t.Date.Month >= MinBound && t.Date.Month <= MaxBound && t.Date.Year == today.Year, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.Year:
                    GetTransactionsTask = _dataProjectionService.GetAllByYearAsync(today.Year, order: DataProjectionService.Order.OrderByDate);
                    break;

                case GlobalResources.TimePeriodsEnum.Today:
                    GetTransactionsTask = _dataProjectionService.GetAllByDateAsync(today, order: DataProjectionService.Order.OrderByDate);
                    break;

                default:
                    GetTransactionsTask = _dataProjectionService.GetAllAsync(order: DataProjectionService.Order.OrderByDate);
                    break;
            }

            Transactions = new(await GetTransactionsTask);
        }

    }
}
