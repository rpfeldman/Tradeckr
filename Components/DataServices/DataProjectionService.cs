using DomainModel;
using Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static DataServices.DataProjectionService;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataServices
{
    public sealed class DataProjectionService(IStateStorage<TransactionDto> StateStorage)
    {
        private IStateStorage<TransactionDto> _StateStorage = StateStorage;
        public enum Order
        {
            OrderByDate, OrderByDateDescending, OrderByValue
        }
        private async Task<OperationResult<List<TransactionDto>>> GetOrdererTransactionsAsync(Expression<Func<TransactionDto, bool>> predicate, Order order)
        {
            var GetEntitiesOperation = await _StateStorage.GetEntitiesAsync(predicate);

            if (!GetEntitiesOperation.Success)
            {
                return GetEntitiesOperation;
            }
            List<TransactionDto> transactions = GetEntitiesOperation.Result!;

            switch (order)
            {
                case Order.OrderByDate:
                    transactions = [.. transactions.OrderBy(t => t.Date)];
                    break;

                case Order.OrderByValue:
                    transactions = [.. transactions.OrderBy(t => t.Value)];
                    break;

                case Order.OrderByDateDescending:
                    transactions = [.. transactions.OrderByDescending(t => t.Date)];
                    break;

                default: break;
            }

            return OperationResult<List<TransactionDto>>.SuccessfulOperation(transactions);
        }
        public static decimal GetSummedTransactions(List<TransactionDto> transactions)
        {
            decimal result = 0;

            foreach (var item in transactions)
            {
                result += item.Value;
            }

            return result;
        }
        public async Task<Option<TransactionDto>> GetTransactionAsync(int TransactionId)
        {
            return await _StateStorage.GetEntityAsync(TransactionId);
        }

        #region All transactions data projection
        public async Task<OperationResult<List<TransactionDto>>> GetAllAsync(bool? IsExpense = null, Order? order = null)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t is TransactionDto, order ?? Order.OrderByValue);
            }
            return await GetOrdererTransactionsAsync(t => t is TransactionDto && t.Depletion == IsExpense, order ?? Order.OrderByValue);
        }
        public async Task<OperationResult<List<TransactionDto>>> GetAllByDateAsync(DateOnly date, bool? IsExpense = null, Order order = Order.OrderByDate) // If you want to get all regardless of whether it's an expense or income, leave 'IsExpense' as null
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Date == date, order);
            }

            return await GetOrdererTransactionsAsync(t => t.Date == date && t.Depletion == IsExpense, order);
        }
        public async Task<OperationResult<List<TransactionDto>>> GetAllByMonthAsync(int month, int year, bool? IsExpense = null, Order order = Order.OrderByDate)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Date.Month == month && t.Date.Year == year, order);
            }

            return await GetOrdererTransactionsAsync(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == IsExpense, order);
        }
        public async Task<OperationResult<List<TransactionDto>>> GetAllByYearAsync(int year, bool? IsExpense = null, Order order = Order.OrderByDate)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Date.Year == year, order);
            }

            return await GetOrdererTransactionsAsync(t => t.Date.Year == year && t.Depletion == IsExpense, order);
        }
        public async Task<OperationResult<List<TransactionDto>>> GetAllByCategoryAsync(string category, bool? IsExpense = null, Order order = Order.OrderByDate)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Category == category, order);
            }

            return await GetOrdererTransactionsAsync(t => t.Category == category && t.Depletion == IsExpense, order);
        }
        public async Task<OperationResult<List<TransactionDto>>> GetAllByPredicateAsync(Expression<Func<TransactionDto, bool>> predicate, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactionsAsync(predicate, order);
        }
        #endregion
    }
}
