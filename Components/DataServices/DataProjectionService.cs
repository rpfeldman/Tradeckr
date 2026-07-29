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
        private async Task<OperationResult<IEnumerable<TransactionDto>>> GetOrdererTransactionsAsync(Expression<Func<TransactionDto, bool>> predicate, Order order)
        {
            var GetEntitiesOperation = await _StateStorage.GetEntitiesAsync(predicate);

            if (!GetEntitiesOperation.Success)
            {
                return GetEntitiesOperation;
            }
            IEnumerable<TransactionDto> transactions = GetEntitiesOperation.Result!;

            switch (order)
            {
                case Order.OrderByDate:
                    transactions = transactions.OrderBy(t => t.Date);
                    break;

                case Order.OrderByValue:
                    transactions = transactions.OrderBy(t => t.Value);
                    break;

                case Order.OrderByDateDescending:
                    transactions = transactions.OrderByDescending(t => t.Date);
                    break;

                default: break;
            }

            return OperationResult<IEnumerable<TransactionDto>>.SuccessfulOperation(transactions);
        }
        private async Task<OperationResult<IEnumerable<TransactionDto>>> GetOrdererTransactionsAsync(Order order)
        {
            var GetEntitiesOperation = await _StateStorage.GetAllAsync();

            if (!GetEntitiesOperation.Success)
            {
                return GetEntitiesOperation;
            }
            IEnumerable<TransactionDto> transactions = GetEntitiesOperation.Result!;

            switch (order)
            {
                case Order.OrderByDate:
                    transactions = transactions.OrderBy(t => t.Date);
                    break;

                case Order.OrderByValue:
                    transactions = transactions.OrderBy(t => t.Value);
                    break;

                case Order.OrderByDateDescending:
                    transactions = transactions.OrderByDescending(t => t.Date);
                    break;

                default: break;
            }

            return OperationResult<IEnumerable<TransactionDto>>.SuccessfulOperation(transactions);
        }
        public static decimal GetSummedTransactions(IEnumerable<TransactionDto> transactions)
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
        public async Task<OperationResult<IEnumerable<TResult>>> ProjectTransactions<TResult>(Expression<Func<TransactionDto, TResult>> Selector)
        {
            var projectedTransactions = await _StateStorage.ProjectAsync<TResult>(Selector);

            return projectedTransactions;
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllAsync(bool? IsExpense = null, Order? order = null)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(order ?? Order.OrderByValue);
            }
            return await GetOrdererTransactionsAsync(t => t is TransactionDto && t.Depletion == IsExpense, order ?? Order.OrderByValue);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByDateAsync(DateOnly date, bool? IsExpense = null, Order order = Order.OrderByDate) // If you want to get all regardless of whether it's an expense or income, leave 'IsExpense' as null
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Date == date, order);
            }

            return await GetOrdererTransactionsAsync(t => t.Date == date && t.Depletion == IsExpense, order);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByMonthAsync(int month, int year, bool? IsExpense = null, Order order = Order.OrderByDate)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Date.Month == month && t.Date.Year == year, order);
            }

            return await GetOrdererTransactionsAsync(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == IsExpense, order);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByYearAsync(int year, bool? IsExpense = null, Order order = Order.OrderByDate)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Date.Year == year, order);
            }

            return await GetOrdererTransactionsAsync(t => t.Date.Year == year && t.Depletion == IsExpense, order);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByCategoryAsync(string category, bool? IsExpense = null, Order order = Order.OrderByDate)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Category == category, order);
            }

            return await GetOrdererTransactionsAsync(t => t.Category == category && t.Depletion == IsExpense, order);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByPredicateAsync(Expression<Func<TransactionDto, bool>> predicate, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactionsAsync(predicate, order);
        }
        #endregion
    }
}
