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
using System.Linq.Expressions;
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
        public partial IEnumerable<GraphableTransactionDto> ExpensesLog { get; set; } = [];

        [ObservableProperty]
        public partial IEnumerable<GraphableTransactionDto> LossesLog { get; set; } = [];

        [ObservableProperty]
		public partial IEnumerable<GraphableTransactionDto> IncomeLog { get; set; } = [];

		[ObservableProperty]
		public partial IEnumerable<GraphableTransactionDto> TransactionsLog { get; set; } = [];

        async partial void OnPickedTimePeriodChanged(KeyValuePair<GlobalResources.TimePeriodsEnum, string> value)
        {
            await ReFillGraphs(value.Key);
        }
		public async Task ReFillGraphs(GlobalResources.TimePeriodsEnum timePeriod)
		{
            Task<OperationResult<IEnumerable<GraphableTransactionDto>>>? GetExpensesTask = null;
            Task<OperationResult<IEnumerable<GraphableTransactionDto>>>? GetIncomeTask = null;
            Task<OperationResult<IEnumerable<GraphableTransactionDto>>>? GetLossesTask = null;
			var today = DateOnly.FromDateTime(DateTime.Today);
			Task<OperationResult<IEnumerable<GraphableTransactionDto>>>[] Predicates = [];


			void SetTasksPointers(Task<OperationResult<IEnumerable<GraphableTransactionDto>>> getExpensesTask, Task<OperationResult<IEnumerable<GraphableTransactionDto>>> getLossesTask, Task< OperationResult<IEnumerable<GraphableTransactionDto>>> getIncomeTask) 
			{
				GetExpensesTask = getExpensesTask;
                GetLossesTask = getLossesTask;
                GetIncomeTask = getIncomeTask;
			}

			Expression<Func<TransactionDto, GraphableTransactionDto>> selector = t => new(t.Depletion ? (t.Value * -1) : t.Value, t.Category, t.Date);
			Predicate<TransactionDto> isExpense = (t) => t.Category != DefaultCategories.TradingCategoryName;

            switch (timePeriod)
			{
				case GlobalResources.TimePeriodsEnum.Historical:
					Predicates =
					[
						
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && !isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false),
                    ];

					break;

				case GlobalResources.TimePeriodsEnum.HistoricalToday:
					Predicates =
					[
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date <= today && isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date <= today && !isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date <= today),
                    ];

					break;

				case GlobalResources.TimePeriodsEnum.Month:
					Predicates =
					[
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month == today.Month && t.Date.Year == today.Year && isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month == today.Month && t.Date.Year == today.Year && !isExpense(t)),
						_dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Month == today.Month && t.Date.Year == today.Year),
					];

					break;

				case GlobalResources.TimePeriodsEnum.ThirtyDays:
					Predicates =
					[
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector,  t => t.Depletion == true && t.Date.DayOfYear >= (today.DayOfYear - 30) && t.Date <= today && t.Date.Year == today.Year && isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector,  t => t.Depletion == true && t.Date.DayOfYear >= (today.DayOfYear - 30) && t.Date <= today && t.Date.Year == today.Year && !isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector,  t => t.Depletion == false && t.Date.DayOfYear >= (today.DayOfYear - 30) && t.Date <= today && t.Date.Year == today.Year),
					];

					break;

				case GlobalResources.TimePeriodsEnum.ThreeMonths:
					Predicates =
					[
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month >= (today.Month-3) && t.Date.Month <= today.Month && t.Date.Year == today.Year && isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month >= (today.Month-3) && t.Date.Month <= today.Month && t.Date.Year == today.Year && !isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Month >= (today.Month-3) && t.Date.Month <= today.Month && t.Date.Year == today.Year),
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
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month >= MinBound && t.Date.Month <= MaxBound && t.Date.Year == today.Year && isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month >= MinBound && t.Date.Month <= MaxBound && t.Date.Year == today.Year && !isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Month >= MinBound && t.Date.Month <= MaxBound && t.Date.Year == today.Year),
                    ];

					break;

				case GlobalResources.TimePeriodsEnum.Year:
					Predicates =
					[
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Year == today.Year && isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Year == today.Year && !isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Year == today.Year),
                    ];

					break;

				case GlobalResources.TimePeriodsEnum.Today:
                    Predicates =
                    [
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date == today && isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date == today && !isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date == today),
                    ];

                    break;

				default:
					Predicates =
					[
						_dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && !isExpense(t)),
                        _dataProjectionService.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false),
                    ];

					break;
			}

			SetTasksPointers(Predicates[0], Predicates[1], Predicates[2]);

			var TaskResults = await Task.WhenAll(GetExpensesTask ?? throw new InvalidOperationException($"{nameof(GetExpensesTask)} doesn't point to a valid task"), GetLossesTask ?? throw new InvalidOperationException($"{nameof(GetLossesTask)} doesn't point to a valid task"), GetIncomeTask ?? throw new InvalidOperationException($"{nameof(GetIncomeTask)} doesn't point to a valid task"));

            if (TaskResults[0].Success)
            {
                ExpensesLog = TaskResults[0].Result!;
            }
            if (TaskResults[1].Success)
            {
                LossesLog = TaskResults[1].Result!;
            }
            if (TaskResults[2].Success)
            {
                IncomeLog = TaskResults[2].Result!;
            }

			TransactionsLog = ExpensesLog.Concat(LossesLog).Concat(IncomeLog);

			return;
		}

		[RelayCommand]
		public async Task ReLoad()
		{
            var getCategoriesOperation = await _categoryPersistenceService.GetCategoriesAsync();

            if (getCategoriesOperation.Success)
            {
                Categories = new(getCategoriesOperation.Result!);
            }
            else { await Shell.Current.DisplayAlertAsync("Error", getCategoriesOperation.InnerError?.ErrorMessage, "Aceptar"); }

            PickedTimePeriod = GlobalResources.TimePeriods.Where(d => d.Key == GlobalResources.TimePeriodsEnum.Month).First();

			await ReFillGraphs(PickedTimePeriod.Key);
        }
    }
}
