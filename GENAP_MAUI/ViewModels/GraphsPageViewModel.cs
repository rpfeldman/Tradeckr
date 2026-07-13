using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using GENAP_MAUI.InnerComponents;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Maui.Platform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
	public sealed partial class GraphsPageViewModel : BaseViewModel
	{
		private DataProjectionService _dataProjectionService;
		private CategoryPersistenceService _categoryPersistenceService;
		public GraphsPageViewModel(DataProjectionService dataProjectionService, CategoryPersistenceService categoryPersistenceService)
		{
			_dataProjectionService = dataProjectionService;
			_categoryPersistenceService = categoryPersistenceService;

			PickedTimePeriod = GlobalResources.TimePeriods.Where(d => d.Key == GlobalResources.TimePeriodsEnum.Month).First();
		}

		[ObservableProperty]
		public partial KeyValuePair<GlobalResources.TimePeriodsEnum, string> PickedTimePeriod { get; set;  }

		[ObservableProperty]
		public partial ObservableCollection<CategoryDto> Categories { get; set; } = new();

        [ObservableProperty]
        public partial List<TransactionDto> ExpensesLog { get; set; } = [];

		[ObservableProperty]
		public partial List<TransactionDto> IncomeLog { get; set; } = [];

		[ObservableProperty]
		public partial List<TransactionDto> TransactionsLog { get; set; } = [];

		[ObservableProperty]
		public partial decimal Expenses { get; set; }

        [ObservableProperty]
        public partial decimal Income { get; set; }


        async partial void OnPickedTimePeriodChanged(KeyValuePair<GlobalResources.TimePeriodsEnum, string> value)
        {
            await ReFillGraphs(value.Key);
        }
		public async Task ReFillGraphs(GlobalResources.TimePeriodsEnum timePeriod)
		{
            Task<OperationResult<List<TransactionDto>>>? GetExpensesTask = null;
            Task<OperationResult<List<TransactionDto>>>? GetIncomeTask = null;
            Task<OperationResult<List<TransactionDto>>>? GetTransactionsTask = null;
			var today = DateOnly.FromDateTime(DateTime.Today);
			Task<OperationResult<List<TransactionDto>>>[] Predicates = [];


			void SetTasksPointers(Task<OperationResult<List<TransactionDto>>> getExpensesTask, Task<OperationResult<List<TransactionDto>>> getIncomeTask, Task<OperationResult<List<TransactionDto>>> getTransactionsTask) 
			{
				GetExpensesTask = getExpensesTask;
				GetIncomeTask = getIncomeTask;
				GetTransactionsTask = getTransactionsTask;
			}

			switch (timePeriod)
			{
				case GlobalResources.TimePeriodsEnum.Historical:
					Predicates =
					[
						_dataProjectionService.GetAllAsync(true),
						_dataProjectionService.GetAllAsync(false),
						_dataProjectionService.GetAllAsync()
					];

					break;

				case GlobalResources.TimePeriodsEnum.HistoricalToday:
					Predicates =
					[
						_dataProjectionService.GetAllByPredicateAsync(t => t.Depletion == true && t.Date <= today),
						_dataProjectionService.GetAllByPredicateAsync(t => t.Depletion == false && t.Date <= today),
						_dataProjectionService.GetAllByPredicateAsync(t => t.Date <= today)
					];

					break;

				case GlobalResources.TimePeriodsEnum.Month:
					Predicates =
					[
						_dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, true),
						_dataProjectionService.GetAllByMonthAsync(today.Month, today.Year, false),
						_dataProjectionService.GetAllByMonthAsync(today.Month, today.Year)
					];

					break;

				case GlobalResources.TimePeriodsEnum.ThirtyDays:
					Predicates =
					[
						_dataProjectionService.GetAllByPredicateAsync(t => t.Depletion == true && t.Date.DayOfYear >= (today.DayOfYear - 30) && t.Date <= today && t.Date.Year == today.Year),
						_dataProjectionService.GetAllByPredicateAsync(t => t.Depletion == false && t.Date.DayOfYear >= (today.DayOfYear - 30) && t.Date <= today && t.Date.Year == today.Year),
						_dataProjectionService.GetAllByPredicateAsync(t => t.Date.DayOfYear >= (today.DayOfYear - 30) && t.Date <= today && t.Date.Year == today.Year)
					];

					break;

				case GlobalResources.TimePeriodsEnum.ThreeMonths:
					Predicates =
					[
						_dataProjectionService.GetAllByPredicateAsync(t => t.Depletion == true && t.Date.Month >= (today.Month-3) && t.Date.Month <= today.Month && t.Date.Year == today.Year),
						_dataProjectionService.GetAllByPredicateAsync(t => t.Depletion == false && t.Date.Month >= (today.Month - 3) && t.Date.Month <= today.Month && t.Date.Year == today.Year),
						_dataProjectionService.GetAllByPredicateAsync(t => t.Date.Month >= (today.Month - 3) && t.Date.Month <= today.Month && t.Date.Year == today.Year)
					];

					break;

				case GlobalResources.TimePeriodsEnum.Semester:
					int MinBound;
					int MaxBound;

					if (today.Month > 6)
					{ MinBound = 7; MaxBound = 12; }
					else { MinBound = 1; MaxBound = 6; }

					Predicates =
					[
						_dataProjectionService.GetAllByPredicateAsync(t => t.Depletion == true && t.Date.Month >= MinBound && t.Date.Month <= MaxBound && t.Date.Year == today.Year),
						_dataProjectionService.GetAllByPredicateAsync(t => t.Depletion == false && t.Date.Month >= MinBound && t.Date.Month <= MaxBound && t.Date.Year == today.Year),
						_dataProjectionService.GetAllByPredicateAsync(t => t.Date.Month >= MinBound && t.Date.Month <= MaxBound && t.Date.Year == today.Year)
					];

					break;

				case GlobalResources.TimePeriodsEnum.Year:
					Predicates =
					[
						_dataProjectionService.GetAllByYearAsync(today.Year, true),
						_dataProjectionService.GetAllByYearAsync(today.Year, false),
						_dataProjectionService.GetAllByYearAsync(today.Year)
					];

					break;

				case GlobalResources.TimePeriodsEnum.Today:
                    Predicates =
                    [
                        _dataProjectionService.GetAllByDateAsync(today, true),
                        _dataProjectionService.GetAllByDateAsync(today, false),
                        _dataProjectionService.GetAllByDateAsync(today),
                    ];

                    break;

				default:
					Predicates =
					[
						_dataProjectionService.GetAllAsync(true),
						_dataProjectionService.GetAllAsync(false),
						_dataProjectionService.GetAllAsync()
					];

					break;
			}

			SetTasksPointers(Predicates[0], Predicates[1], Predicates[2]);

			var TaskResults = await Task.WhenAll(GetExpensesTask ?? throw new InvalidOperationException($"{nameof(GetExpensesTask)} doesn't point to a valid task"), GetIncomeTask ?? throw new InvalidOperationException($"{nameof(GetIncomeTask)} doesn't point to a valid task"), GetTransactionsTask ?? throw new InvalidOperationException($"{nameof(GetTransactionsTask)} doesn't point to a valid task"));

            if (TaskResults[0].Success)
            {
                ExpensesLog = TaskResults[0].Result!;
            }
            if (TaskResults[1].Success)
            {
                IncomeLog = TaskResults[1].Result!;
            }
            if (TaskResults[2].Success)
            {
                TransactionsLog = TaskResults[2].Result!;
            }

            Expenses = DataProjectionService.GetSummedTransactions(ExpensesLog);
			Income = DataProjectionService.GetSummedTransactions(IncomeLog);

			return;
		}

		[RelayCommand]
		public async Task ReLoad()
		{
            PickedTimePeriod = GlobalResources.TimePeriods.Where(d => d.Key == GlobalResources.TimePeriodsEnum.Month).First();

			await ReFillGraphs(PickedTimePeriod.Key);
        }
    }
}
