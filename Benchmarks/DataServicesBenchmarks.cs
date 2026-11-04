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
using Microsoft.Data.Sqlite;

namespace Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByParams)]
    [RankColumn]
    public class DataServicesBenchmarks
    {
        private const string TemplateDbPath = "template.db"; 
        private const string WorkingDbPath  = "benchmark.db"; 

        private readonly EF_SQLite_StateStorageRepo<TransactionDto> _templateRepo;
        private readonly EF_SQLite_StateStorageRepo<TransactionDto> _workingRepo;
        private readonly DataRegistrationService _dataRegistrationService;
        private readonly Methods methods;

        [ParamsSource(nameof(paramSource))]
        public (string, string) Names_Param { get; set; }

        public IEnumerable<(string, string)> paramSource()
        {
            yield return ("Food", "Comida");
            yield return ("Clothing", "Ropa");
            yield return ("Subscriptions", "Suscripciones");
        }

        public DataServicesBenchmarks()
        {
            Batteries_V2.Init();

            SqliteConnection.ClearAllPools();
            DeleteDatabase(TemplateDbPath);
            DeleteDatabase(WorkingDbPath);

            _templateRepo = new(TemplateDbPath);
            _workingRepo  = new(WorkingDbPath);

            _dataRegistrationService = new(_templateRepo); 
            methods = new(_workingRepo);                   
        }

        [GlobalSetup]
        public async Task Setup()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var random = new Random(42); 

            string[] categories = ["Food", "Clothing", "Subscriptions"];

            foreach (var category in categories)
            {
                for (var i = 0; i < 500; i++)
                {
                    var value = random.Next(1_000, 300_000);
                    var date = today.AddDays(-random.Next(0, 365));

                    if (i % 4 == 0)
                        await _dataRegistrationService.RegistIncomeAsync(value, date, category);
                    else
                        await _dataRegistrationService.RegistExpenseAsync(value, date, category);
                }
            }

            SqliteConnection.ClearAllPools();
        }

        [IterationSetup]
        public void ResetDatabase()
        {
            SqliteConnection.ClearAllPools();
            DeleteDatabase(WorkingDbPath);
            File.Copy(TemplateDbPath, WorkingDbPath, overwrite: true);
        }

        [Benchmark]
        [InvocationCount(1)]
        public async Task<OperationResult> RenameCategory_RowByRow()
        {
            return await methods.RowByRowRenameCategoryAsync(Names_Param.Item1, Names_Param.Item2);
        }

        [Benchmark]
        [InvocationCount(1)]
        public async Task<OperationResult<int>> RenameCategory_SetBased()
        {
            return await methods.SetBasedRenameCategoryAsync(Names_Param.Item1, Names_Param.Item2);
        }

        private static void DeleteDatabase(string path)
        {
            File.Delete(path);
            File.Delete(path + "-wal");
            File.Delete(path + "-shm");
        }
    }

    public sealed class Methods(IStateStorage<TransactionDto> _StateStorage)
    {
        public async Task<OperationResult> RowByRowRenameCategoryAsync(string OldName, string NewName)
        {
            var anyTransactionOperation = await _StateStorage.AnyAsync(t => t.Category == OldName);

            if (!anyTransactionOperation.Success)
            {
                return OperationResult.FaultedOperation(anyTransactionOperation.InnerError);
            }
            if (!anyTransactionOperation.Result)
            {
                return OperationResult.SuccessfulOperation(); // Not an error: no transactions had that name, so there was nothing to rename
            }

            var getOldTransactionsOperation = await _StateStorage.GetEntitiesAsync(t => t.Category == OldName);
            
            if(!getOldTransactionsOperation.Success) { return OperationResult.FaultedOperation(getOldTransactionsOperation.InnerError); }

            var transactions = getOldTransactionsOperation.Result!;

            foreach (var transaction in transactions)
            {
                transaction.Category = NewName;
            }

            var updateRangeOperation = await _StateStorage.UpdateRangeAsync(transactions.ToArray());

            if (updateRangeOperation.Success)
            {
                if (updateRangeOperation.Result != transactions.Count())
                {
                    return OperationResult.FaultedOperation(ServiceErrors.PartialRegistrationError("movements", "renamed"));
                }

                return OperationResult.SuccessfulOperation();
            }

            return OperationResult.FaultedOperation(updateRangeOperation.InnerError);
        } 

         public async Task<OperationResult<int>> SetBasedRenameCategoryAsync(string OldName, string NewName)
         {
              return await _StateStorage.UpdateRangeAsync(t => t.Category == OldName, s => s.SetProperty(t => t.Category, NewName));
         } 
    }
}
