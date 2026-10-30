using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using GENAP_MAUI.CustomViews;
using GENAP_MAUI.InnerComponents;
using GENAP_MAUI.Resources.Strings;
using Serilog;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using static GENAP_MAUI.GlobalResources;

namespace GENAP_MAUI.ViewModels
{
	public sealed partial class GraphsPageViewModel : BaseViewModel
	{
		private DataProjectionService _dataProjectionService;
		private CategoryPersistenceService _categoryPersistenceService;

        private CustomDialogsService _customDialogsService;
        private bool _IsAlreadyFillingGraphs;
		public GraphsPageViewModel(DataProjectionService dataProjectionService, CategoryPersistenceService categoryPersistenceService, CustomDialogsService customDialogsService)
		{
			_dataProjectionService = dataProjectionService;
			_categoryPersistenceService = categoryPersistenceService;
            _customDialogsService = customDialogsService;
        }

		[ObservableProperty]
		public partial KeyValuePair<GlobalResources.TimePeriodsEnum, string> PickedTimePeriod { get; set;  }

        [ObservableProperty]
        public partial CurrencyDto PickedCurrency { get; set; } 

        [ObservableProperty]
		public partial ObservableCollection<CategoryDto> Categories { get; set; } = new();

        [ObservableProperty]
        public partial IEnumerable<GraphableTransactionDto> ExpensesLog { get; set; } = [];

        [ObservableProperty]
        public partial IEnumerable<GraphableTransactionDto> LossesLog { get; set; } = [];

        [ObservableProperty]
		public partial IEnumerable<GraphableTransactionDto> IncomeLog { get; set; } = [];

        [ObservableProperty]
        public partial IEnumerable<GraphableTransactionDto> ProfitLog { get; set; } = [];

        [ObservableProperty]
		public partial IEnumerable<GraphableTransactionDto> TransactionsLog { get; set; } = [];

        [ObservableProperty]
        public partial IEnumerable<GraphableTransactionDto> TradingLog { get; set; } = [];

        async partial void OnPickedTimePeriodChanged(KeyValuePair<GlobalResources.TimePeriodsEnum, string> value)
        {
            if (_IsAlreadyFillingGraphs) { return; }
            await ReFillGraphs(value.Key, PickedCurrency);
        }

        async partial void OnPickedCurrencyChanged(CurrencyDto value)
        { 
            if (_IsAlreadyFillingGraphs) { return; }
            await ReFillGraphs(PickedTimePeriod.Key, value);
        }

		public async Task ReFillGraphs(GlobalResources.TimePeriodsEnum timePeriod, CurrencyDto currency)
		{
            var today = DateOnly.FromDateTime(DateTime.Today);

            Expression<Func<TransactionDto, GraphableTransactionDto>> selector = 
                t => new
                (
                    t.Depletion ? CurrencyConverterService.TfuToCurrency(t.Value, currency) * -1 : CurrencyConverterService.TfuToCurrency(t.Value, currency), t.Category, t.Date
                );

            void FilterTransactions(IEnumerable<GraphableTransactionDto> transactions)
            {
                var length = transactions.Count();

                List<GraphableTransactionDto> expenses = new(length);
                List<GraphableTransactionDto> losses = new(length);
                List<GraphableTransactionDto> income = new(length);
                List<GraphableTransactionDto> profit = new(length);

                foreach (var transaction in transactions)
                {
                    if(transaction.SignedValue < 0)
                    {
                        if(transaction.Category == DefaultCategories.TradingCategoryName) { losses.Add(transaction); continue; }

                        expenses.Add(transaction);
                        continue;
                    }

                    if(transaction.Category == DefaultCategories.TradingCategoryName) { profit.Add(transaction); continue; }

                    income.Add(transaction);
                }

                ExpensesLog = expenses.AsEnumerable();
                LossesLog = losses.AsEnumerable();
                IncomeLog = income.AsEnumerable();
                ProfitLog = profit.AsEnumerable();

                TransactionsLog = ExpensesLog.Concat(LossesLog).Concat(IncomeLog);
                TradingLog = LossesLog.Concat(ProfitLog);
            }

            OperationResult<IEnumerable<GraphableTransactionDto>> getTransactionOperation;

            switch (timePeriod)
            {
                case TimePeriodsEnum.Historical:
                    getTransactionOperation = await _dataProjectionService.ProjectTransactions(selector);

                    if (!getTransactionOperation.Success)
                    {
                        await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, getTransactionOperation.InnerError!.ErrorMessage, AppResources.Accept_AlertButton);

                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.HistoricalToday:
                    getTransactionOperation = await _dataProjectionService.ProjectTransactions(selector, t => t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, getTransactionOperation.InnerError!.ErrorMessage, AppResources.Accept_AlertButton);

                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Month:
                    getTransactionOperation = await _dataProjectionService.ProjectTransactions(selector, t => t.Date.Month == today.Month && t.Date.Year == today.Year);

                    if (!getTransactionOperation.Success)
                    {
                        await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, getTransactionOperation.InnerError!.ErrorMessage, AppResources.Accept_AlertButton);

                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                break;

                case TimePeriodsEnum.ThirtyDays:
                    getTransactionOperation = await _dataProjectionService.ProjectTransactions(selector, t => t.Date.DayNumber >= (today.DayNumber - 30) && t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, getTransactionOperation.InnerError!.ErrorMessage, AppResources.Accept_AlertButton);

                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.ThreeMonths:
                    getTransactionOperation = await _dataProjectionService.ProjectTransactions(selector, t => t.Date.DayNumber >= (today.DayNumber - 90) && t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, getTransactionOperation.InnerError!.ErrorMessage, AppResources.Accept_AlertButton);

                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Semester:
                    int MinBound = today.Month > 6 ? 7 : 1;

                    getTransactionOperation = await _dataProjectionService.ProjectTransactions(selector, t => t.Date.Month >= MinBound && t.Date <= today && t.Date.Year == today.Year);

                    if (!getTransactionOperation.Success)
                    {
                        await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, getTransactionOperation.InnerError!.ErrorMessage, AppResources.Accept_AlertButton);

                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Year:
                    getTransactionOperation = await _dataProjectionService.ProjectTransactions(selector, t => t.Date.Year == today.Year && t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, getTransactionOperation.InnerError!.ErrorMessage, AppResources.Accept_AlertButton);

                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Today:
                    getTransactionOperation = await _dataProjectionService.ProjectTransactions(selector, t => t.Date == today);

                    if (!getTransactionOperation.Success)
                    {
                        await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, getTransactionOperation.InnerError!.ErrorMessage, AppResources.Accept_AlertButton);

                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                default:
                    break;
            }

            Log.Information($"Charts filled with new data using timeperiod '{timePeriod}' and currency '{currency.IsoCode}' (GraphsPage) ");

			return;
		}

		[RelayCommand]
		public async Task ReLoad()
		{
            _IsAlreadyFillingGraphs = true;

            var getCategoriesOperation = await _categoryPersistenceService.GetCategoriesAsync();
                getCategoriesOperation.WriteLog("Bring categories from storage (GraphsPage)");

            if (getCategoriesOperation.Success)
            {
                Categories = new(getCategoriesOperation.Result!);
            }
            else { await _customDialogsService.ShowAlertDialogAsync(AppResources.Error_AlertTitle, getCategoriesOperation.InnerError?.ErrorMessage!, AppResources.Accept_AlertButton); }

            PickedCurrency = GlobalResources.DefaultCommonCurrency;
            PickedTimePeriod = GlobalResources.TimePeriodsList[1];

            await ReFillGraphs(TimePeriodsEnum.ThirtyDays, PickedCurrency);
            
            _IsAlreadyFillingGraphs = false;
        }
    }
}
