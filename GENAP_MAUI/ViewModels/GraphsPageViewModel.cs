using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using GENAP_MAUI.InnerComponents;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microcharts;
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
		public GlobalResources GlobalResources { get; }

		public GraphsPageViewModel(DataProjectionService dataProjectionService, GlobalResources globalResources)
		{
			_dataProjectionService = dataProjectionService;
			GlobalResources = globalResources;

			Categories = new(GlobalResources.GlobalCategories);

			PickedTimePeriod = GlobalResources.TimePeriods.First();
		}

		[ObservableProperty]
		public partial KeyValuePair<GlobalResources.TimePeriodsEnum, string> PickedTimePeriod { get; set;  }

		[ObservableProperty]
		public partial ObservableCollection<CategoryDto> Categories { get; set; }

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
			try
			{
				await ReFillGraphs(value.Key);
			}
			catch (Exception x)
			{
				// FUTURE: Exception managment

				Console.WriteLine(x);
			}
        }

		[RelayCommand]
        public async Task FillGraphs()
        {
            Categories = new(GlobalResources.GlobalCategories);

            var GetExpensesTask = _dataProjectionService.GetAllAsync(true);
			var GetIncomeTask = _dataProjectionService.GetAllAsync(false);
			var GetTransactionsTask = _dataProjectionService.GetAllAsync();

			var TaskResults = await Task.WhenAll(GetExpensesTask, GetIncomeTask, GetTransactionsTask);

            ExpensesLog = [.. TaskResults[0]];
			IncomeLog = [.. TaskResults[1]];
			TransactionsLog = [.. TaskResults[2]];

            Expenses = DataProjectionService.GetSummedTransactions(ExpensesLog);
            Income = DataProjectionService.GetSummedTransactions(IncomeLog);

            return;
		}

		public async Task ReFillGraphs(GlobalResources.TimePeriodsEnum timePeriod)
		{
            Task<List<TransactionDto>>? GetExpensesTask = null;
			Task<List<TransactionDto>>? GetIncomeTask = null;
			Task<List<TransactionDto>>? GetTransactionsTask = null;
			var today = DateOnly.FromDateTime(DateTime.Today);
			Task<List<TransactionDto>>[] Predicates = [];


			void SetTasksPointers(Task<List<TransactionDto>> getExpensesTask, Task<List<TransactionDto>> getIncomeTask, Task<List<TransactionDto>> getTransactionsTask) 
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

			ExpensesLog = [.. TaskResults[0]];
			IncomeLog = [.. TaskResults[1]];
			TransactionsLog = [.. TaskResults[2]];

			Expenses = DataProjectionService.GetSummedTransactions(ExpensesLog);
			Income = DataProjectionService.GetSummedTransactions(IncomeLog);

			return;
		}
    }
}
