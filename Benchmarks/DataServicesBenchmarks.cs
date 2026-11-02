using BenchmarkDotNet.Attributes;
using DataServices;
using DomainModel;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SQLitePCL;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace Benchmarks
{
    public sealed record GraphableTransactionDto(decimal Value, string Category, DateOnly Date);
    public enum TimePeriodsEnum { Historical, HistoricalToday, Month, ThirtyDays, ThreeMonths, Semester, Year, Today };

    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByParams)]
    [RankColumn]
    public class DataServicesBenchmarks 
    {
        private readonly EF_SQLite_StateStorageRepo<TransactionDto> _repo;
        private readonly DataProjectionService _dataProjectionService;
        private readonly DataRegistrationService _dataRegistrationService;

        private readonly Methods GraphsPageMethods;

        public DataServicesBenchmarks()
        {
            Batteries_V2.Init();

            _repo = new("BencharmStorage.db");
            _dataProjectionService = new(_repo);
            _dataRegistrationService = new(_repo);

            GraphsPageMethods = new(_dataProjectionService);
        }

        public CurrencyDto currencyParam = new() { ConversionRate = 1, CurrencyDisplayName = "Test currency", IsoCode = "TFU", Id = 0};

        [Params(TimePeriodsEnum.Year, TimePeriodsEnum.Historical, TimePeriodsEnum.Month)]
        public TimePeriodsEnum timePeriodParam;

        [GlobalSetup]
        public async void Setup()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            await _dataRegistrationService.RegistExpenseAsync(4500m, today.AddDays(-140), "Food");
            await _dataRegistrationService.RegistExpenseAsync(72000m, today.AddDays(-71), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(46000m, today.AddDays(-279), "Food");
            await _dataRegistrationService.RegistExpenseAsync(40500m, today.AddDays(-216), "Food");
            await _dataRegistrationService.RegistExpenseAsync(4500m, today.AddDays(-47), "Food");
            await _dataRegistrationService.RegistExpenseAsync(74500m, today.AddDays(-258), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(38500m, today.AddDays(-101), "Food");
            await _dataRegistrationService.RegistExpenseAsync(9000m, today.AddDays(-229), "Subscriptions");
            await _dataRegistrationService.RegistExpenseAsync(8500m, today.AddDays(-81), "Social");
            await _dataRegistrationService.RegistExpenseAsync(12500m, today.AddDays(-142), "Subscriptions");
            await _dataRegistrationService.RegistExpenseAsync(70000m, today.AddDays(-172), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(8500m, today.AddDays(-194), "Food");
            await _dataRegistrationService.RegistExpenseAsync(25500m, today.AddDays(-176), "Food");
            await _dataRegistrationService.RegistExpenseAsync(13500m, today.AddDays(-235), "Social");
            await _dataRegistrationService.RegistExpenseAsync(27000m, today.AddDays(-40), "Food");
            await _dataRegistrationService.RegistExpenseAsync(88000m, today.AddDays(-316), "Social");
            await _dataRegistrationService.RegistExpenseAsync(81500m, today.AddDays(-98), "Social");
            await _dataRegistrationService.RegistExpenseAsync(5500m, today.AddDays(-338), "Food");
            await _dataRegistrationService.RegistExpenseAsync(89000m, today.AddDays(-40), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(40500m, today.AddDays(-194), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(66000m, today.AddDays(-325), "Social");
            await _dataRegistrationService.RegistExpenseAsync(28500m, today.AddDays(-189), "Social");
            await _dataRegistrationService.RegistExpenseAsync(34500m, today.AddDays(-343), "Social");
            await _dataRegistrationService.RegistExpenseAsync(17000m, today.AddDays(-311), "Social");
            await _dataRegistrationService.RegistExpenseAsync(77500m, today.AddDays(-83), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(14000m, today.AddDays(-138), "Subscriptions");
            await _dataRegistrationService.RegistExpenseAsync(98000m, today.AddDays(-28), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(23000m, today.AddDays(-161), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(10500m, today.AddDays(-33), "Subscriptions");
            await _dataRegistrationService.RegistExpenseAsync(95500m, today.AddDays(-108), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(14500m, today.AddDays(-329), "Subscriptions");
            await _dataRegistrationService.RegistExpenseAsync(6500m, today.AddDays(-135), "Subscriptions");
            await _dataRegistrationService.RegistExpenseAsync(78000m, today.AddDays(-287), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(82500m, today.AddDays(-219), "Social");
            await _dataRegistrationService.RegistExpenseAsync(13500m, today.AddDays(-112), "Subscriptions");
            await _dataRegistrationService.RegistExpenseAsync(145000m, today.AddDays(-252), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(51000m, today.AddDays(-24), "Food");
            await _dataRegistrationService.RegistExpenseAsync(12500m, today.AddDays(-321), "Food");
            await _dataRegistrationService.RegistExpenseAsync(123000m, today.AddDays(-305), "Clothing");
            await _dataRegistrationService.RegistExpenseAsync(27500m, today.AddDays(-195), "Food");

            await _dataRegistrationService.RegistIncomeAsync(1133000m, today.AddDays(-128), "Other");
            await _dataRegistrationService.RegistIncomeAsync(284000m, today.AddDays(-349), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(746000m, today.AddDays(-57), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(940000m, today.AddDays(-80), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(56000m, today.AddDays(-134), "Other");
            await _dataRegistrationService.RegistIncomeAsync(1089000m, today.AddDays(-54), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(1089000m, today.AddDays(-311), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(363000m, today.AddDays(-191), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(1154000m, today.AddDays(-271), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(713000m, today.AddDays(-250), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(279000m, today.AddDays(-185), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(540000m, today.AddDays(-29), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(211000m, today.AddDays(-43), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(191000m, today.AddDays(-272), "Other");
            await _dataRegistrationService.RegistIncomeAsync(312000m, today.AddDays(-337), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(1175000m, today.AddDays(-84), "Other");
            await _dataRegistrationService.RegistIncomeAsync(1130000m, today.AddDays(-310), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(483000m, today.AddDays(-276), "Other");
            await _dataRegistrationService.RegistIncomeAsync(688000m, today.AddDays(-204), "Salary");
            await _dataRegistrationService.RegistIncomeAsync(947000m, today.AddDays(-264), "Salary");

            await _dataRegistrationService.RegistExpenseAsync(236000m, today.AddDays(-61),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(131500m, today.AddDays(-115),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(37500m, today.AddDays(-173),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(15500m, today.AddDays(-301),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(288500m, today.AddDays(-117),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(306000m, today.AddDays(-112),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(8500m, today.AddDays(-36),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(367000m, today.AddDays(-323),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(35000m, today.AddDays(-117),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(39500m, today.AddDays(-16),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(174000m, today.AddDays(-36),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(268000m, today.AddDays(-121),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(147500m, today.AddDays(-342),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(253500m, today.AddDays(-109),  "Trading");
            await _dataRegistrationService.RegistExpenseAsync(281000m, today.AddDays(-67),  "Trading");

            await _dataRegistrationService.RegistIncomeAsync(375000m, today.AddDays(-292),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(300000m, today.AddDays(-242),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(129000m, today.AddDays(-242),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(418000m, today.AddDays(-208),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(102000m, today.AddDays(-48),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(54500m, today.AddDays(-337),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(225500m, today.AddDays(-181),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(221500m, today.AddDays(-210),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(244000m, today.AddDays(-27),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(349500m, today.AddDays(-334),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(335500m, today.AddDays(-50),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(36000m, today.AddDays(-206),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(377500m, today.AddDays(-173),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(414500m, today.AddDays(-55),  "Trading");
            await _dataRegistrationService.RegistIncomeAsync(132000m, today.AddDays(-98),  "Trading");

            await _dataRegistrationService.RegistFixedExpenseAsync(9900m, today.AddDays(-97), "Subscriptions", 12);
            await _dataRegistrationService.RegistFixedExpenseAsync(120000m, today.AddDays(-274), "Food", 6);
            await _dataRegistrationService.RegistFixedExpenseAsync(45000m, today.AddDays(-229), "Social", 4);
            await _dataRegistrationService.RegistFixedExpenseAsync(60000m, today.AddDays(-71), "Clothing", 3);
            await _dataRegistrationService.RegistFixedExpenseAsync(4500m, today.AddDays(-216), "Subscriptions", 10);
            await _dataRegistrationService.RegistFixedIncomeAsync(850000m, today.AddDays(-93), "Salary", 12);
            await _dataRegistrationService.RegistFixedIncomeAsync(300000m, today.AddDays(-142), "Salary", 6);
            await _dataRegistrationService.RegistFixedIncomeAsync(50000m, today.AddDays(-236), "Other", 8);
            await _dataRegistrationService.RegistFixedIncomeAsync(1200000m, today.AddDays(-127), "Salary", 3);
            await _dataRegistrationService.RegistFixedIncomeAsync(25000m, today.AddDays(-38), "Other", 5);
        }

        [Benchmark]
        public async Task OldMethod()
        {
            await GraphsPageMethods.OldFillGraphs(timePeriodParam, currencyParam);
        }

        [Benchmark]
        public async Task OptimizedMethodLINQ()
        {
            await GraphsPageMethods.NewFillGraphsMethodWithLINQ(timePeriodParam, currencyParam);
        }

        [Benchmark]
        public async Task OptimizedMethodManual()
        {
            await GraphsPageMethods.NewFillGraphsMethodWithManualFiltering(timePeriodParam, currencyParam);
        }
    }

    public sealed class Methods(DataProjectionService service)
    {
        public  IEnumerable<GraphableTransactionDto> ExpensesLog { get; set; } = [];
        public  IEnumerable<GraphableTransactionDto> LossesLog { get; set; } = [];
		public IEnumerable<GraphableTransactionDto> IncomeLog { get; set; } = [];
        public IEnumerable<GraphableTransactionDto> ProfitLog { get; set; } = [];
		public IEnumerable<GraphableTransactionDto> TransactionsLog { get; set; } = [];
        public IEnumerable<GraphableTransactionDto> TradingLog { get; set; } = [];
        public async Task NewFillGraphsMethodWithManualFiltering(TimePeriodsEnum timePeriod, CurrencyDto currency)
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
                    if(transaction.Value < 0)
                    {
                        if(transaction.Category == "Trading") { losses.Add(transaction); continue; }

                        expenses.Add(transaction);
                        continue;
                    }

                    if(transaction.Category == "Trading") { profit.Add(transaction); continue; }

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
                    getTransactionOperation = await service.ProjectTransactions(selector);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.HistoricalToday:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Month:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.Month == today.Month && t.Date.Year == today.Year);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                break;

                case TimePeriodsEnum.ThirtyDays:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.DayNumber >= (today.DayNumber - 30) && t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.ThreeMonths:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.DayNumber >= (today.DayNumber - 90) && t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Semester:
                    int MinBound = today.Month > 6 ? 7 : 1;

                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.Month >= MinBound && t.Date <= today && t.Date.Year == today.Year);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Year:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.Year == today.Year && t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Today:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date == today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                default:
                    break;
            }

			return;
		}

        public async Task NewFillGraphsMethodWithLINQ(TimePeriodsEnum timePeriod, CurrencyDto currency)
		{
            var today = DateOnly.FromDateTime(DateTime.Today);

            Expression<Func<TransactionDto, GraphableTransactionDto>> selector = 
                t => new
                (
                    t.Depletion ? CurrencyConverterService.TfuToCurrency(t.Value, currency) * -1 : CurrencyConverterService.TfuToCurrency(t.Value, currency), t.Category, t.Date
                );

            void FilterTransactions(IEnumerable<GraphableTransactionDto> transactions)
            {
                ExpensesLog = transactions.Where(t => t.Value < 0 && t.Category != "Trading");
                LossesLog = transactions.Where(t => t.Value < 0 && t.Category == "Trading");
                IncomeLog = transactions.Where(t => t.Value > 0);
                ProfitLog = transactions.Where(t => t.Value > 0 && t.Category == "Trading");

                TransactionsLog = ExpensesLog.Concat(LossesLog).Concat(IncomeLog);
                TradingLog = LossesLog.Concat(ProfitLog);
            }

            OperationResult<IEnumerable<GraphableTransactionDto>> getTransactionOperation;

            switch (timePeriod)
            {
                case TimePeriodsEnum.Historical:
                    getTransactionOperation = await service.ProjectTransactions(selector);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.HistoricalToday:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Month:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.Month == today.Month && t.Date.Year == today.Year);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                break;

                case TimePeriodsEnum.ThirtyDays:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.DayNumber >= (today.DayNumber - 30) && t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.ThreeMonths:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.DayNumber >= (today.DayNumber - 90) && t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Semester:
                    int MinBound = today.Month > 6 ? 7 : 1;

                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.Month >= MinBound && t.Date <= today && t.Date.Year == today.Year);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Year:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date.Year == today.Year && t.Date <= today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                case TimePeriodsEnum.Today:
                    getTransactionOperation = await service.ProjectTransactions(selector, t => t.Date == today);

                    if (!getTransactionOperation.Success)
                    {
                        return;
                    }

                    FilterTransactions(getTransactionOperation.Result!);

                    break;

                default:
                    break;
            }

			return;
		}
		public async Task OldFillGraphs(TimePeriodsEnum timePeriod, CurrencyDto currency)
		{
            Task<OperationResult<IEnumerable<GraphableTransactionDto>>>? GetExpensesTask = null;
            Task<OperationResult<IEnumerable<GraphableTransactionDto>>>? GetIncomeTask = null;
            Task<OperationResult<IEnumerable<GraphableTransactionDto>>>? GetLossesTask = null;
            Task<OperationResult<IEnumerable<GraphableTransactionDto>>>? GetProfitTask = null;
            var today = DateOnly.FromDateTime(DateTime.Today);
			Task<OperationResult<IEnumerable<GraphableTransactionDto>>>[] Predicates = [];


			void SetTasksPointers(Task<OperationResult<IEnumerable<GraphableTransactionDto>>> getExpensesTask, Task<OperationResult<IEnumerable<GraphableTransactionDto>>> getLossesTask, Task< OperationResult<IEnumerable<GraphableTransactionDto>>> getIncomeTask, Task<OperationResult<IEnumerable<GraphableTransactionDto>>> getProfitTask) 
			{
				GetExpensesTask = getExpensesTask;
                GetLossesTask = getLossesTask;
                GetIncomeTask = getIncomeTask;
				GetProfitTask = getProfitTask;
			}

			Expression<Func<TransactionDto, GraphableTransactionDto>> selector = t => new(t.Depletion ? CurrencyConverterService.TfuToCurrency(t.Value, currency) * -1 : CurrencyConverterService.TfuToCurrency(t.Value, currency), t.Category, t.Date);

            switch (timePeriod)
            {
                case TimePeriodsEnum.Historical:
                    Predicates =
                    [
                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Category !=  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Category ==  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Category ==  "Trading"),
                    ];

                    break;

                case  TimePeriodsEnum.HistoricalToday:
                    Predicates =
                    [
                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date <= today && t.Category !=  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date <= today && t.Category ==  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date <= today),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date <= today && t.Category ==  "Trading"),
                    ];

                    break;

                case  TimePeriodsEnum.Month:
                    Predicates =
                    [
                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month == today.Month && t.Date.Year == today.Year&& t.Category !=  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month == today.Month && t.Date.Year == today.Year && t.Category ==  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Month == today.Month && t.Date.Year == today.Year),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Month == today.Month && t.Date.Year == today.Year && t.Category ==  "Trading"),
                    ];
                    break;

				case  TimePeriodsEnum.ThirtyDays:
					Predicates =
					[
                        service.ProjectTransactions<GraphableTransactionDto>(selector,  t => t.Depletion == true && t.Date.DayNumber >= (today.DayNumber - 30) && t.Date <= today && t.Date.Year == today.Year && t.Category !=  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector,  t => t.Depletion == true && t.Date.DayNumber >= (today.DayNumber - 30) && t.Date <= today && t.Date.Year == today.Year && t.Category ==  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector,  t => t.Depletion == false && t.Date.DayNumber >=(today.DayNumber - 30) && t.Date <= today && t.Date.Year == today.Year),

                        service.ProjectTransactions<GraphableTransactionDto>(selector,  t => t.Depletion == false && t.Date.DayNumber >= (today.DayNumber - 30) && t.Date <= today && t.Date.Year == today.Year&& t.Category ==  "Trading"),
                    ];

					break;

				case  TimePeriodsEnum.ThreeMonths:

					Predicates =
					[
                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.DayNumber >= (today.DayNumber - 90) && t.Date <= today && t.Category !=  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.DayNumber >= (today.DayNumber-90) && t.Date <= today && t.Category ==  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.DayNumber >= (today.DayNumber-90) && t.Date <= today),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.DayNumber >= (today.DayNumber-90) && t.Date <= today && t.Category ==  "Trading"),
                    ];

					break;

				case  TimePeriodsEnum.Semester:
					int MinBound;

					if (today.Month > 6)
                        MinBound = 7;
					else 
                        MinBound = 1; 

					Predicates =
					[
                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month >= MinBound && t.Date <= today && t.Date.Year == today.Year && t.Category !=  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Month >= MinBound && t.Date <= today && t.Date.Year == today.Year && t.Category ==  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Month >= MinBound && t.Date <= today && t.Date.Year == today.Year),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Month >= MinBound && t.Date <= today && t.Date.Year == today.Year && t.Category ==  "Trading"),
                    ];

					break;

				case  TimePeriodsEnum.Year:
					Predicates =
					[
                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Year == today.Year && t.Date <= today && t.Category !=  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date.Year == today.Year && t.Date <= today && t.Category ==  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Year == today.Year && t.Date <= today),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date.Year == today.Year && t.Date <= today && t.Category ==  "Trading"),
                    ];

					break;

				case  TimePeriodsEnum.Today:
                    Predicates =
                    [
                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date == today && t.Category !=  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == true && t.Date == today && t.Category ==  "Trading"),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date == today),

                        service.ProjectTransactions<GraphableTransactionDto>(selector, t => t.Depletion == false && t.Date == today && t.Category ==  "Trading"),
                    ];

                    break;

				default:
                    break;
			}

			SetTasksPointers(Predicates[0], Predicates[1], Predicates[2], Predicates[3]);

			var TaskResults = await Task.WhenAll(GetExpensesTask ?? throw new InvalidOperationException($"{nameof(GetExpensesTask)} doesn't point to a valid task"), GetLossesTask ?? throw new InvalidOperationException($"{nameof(GetLossesTask)} doesn't point to a valid task"), GetIncomeTask ?? throw new InvalidOperationException($"{nameof(GetIncomeTask)} doesn't point to a valid task"), GetProfitTask ?? throw new InvalidOperationException($"{nameof(GetProfitTask)} doesn't point to a valid task"));

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
            if (TaskResults[3].Success)
            {
                ProfitLog = TaskResults[3].Result!;
            }

            TransactionsLog = ExpensesLog.Concat(LossesLog).Concat(IncomeLog);
            TradingLog = LossesLog.Concat(ProfitLog);

			return;
		}
    }   
}
