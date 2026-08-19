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
        private async Task<OperationResult<IEnumerable<TransactionDto>>> GetOrdererTransactionsAsync(Expression<Func<TransactionDto, bool>> predicate, Order order, CurrencyDto? currency = null)
        {
            var GetEntitiesOperation = currency is null ?
                await _StateStorage.GetEntitiesAsync(predicate) :
                await _StateStorage.ProjectByPredicateAsync<TransactionDto>(t => new() { Value = CurrencyConverterService.TfuToCurrency(t.Value, currency!), Category = t.Category, Date = t.Date, Depletion = t.Depletion, Fixed = t.Fixed, Id = t.Id }, predicate);

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
        private async Task<OperationResult<IEnumerable<TransactionDto>>> GetOrdererTransactionsAsync(Order order, CurrencyDto? currency = null)
        {
            var GetEntitiesOperation = currency is null ?
                await _StateStorage.GetAllAsync() :
                await _StateStorage.ProjectAsync<TransactionDto>(t => new() { Value = CurrencyConverterService.TfuToCurrency(t.Value, currency!), Category = t.Category, Date = t.Date, Depletion = t.Depletion, Fixed = t.Fixed, Id = t.Id });
                

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
        public async Task<OperationResult<IEnumerable<TResult>>> ProjectTransactions<TResult>(Expression<Func<TransactionDto, TResult>> Selector, Expression<Func<TransactionDto, bool>> Predicate)
        {
            var projectedTransactions = await _StateStorage.ProjectByPredicateAsync<TResult>(Selector, Predicate);

            return projectedTransactions;
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllAsync(bool? IsExpense = null, Order? order = null, CurrencyDto? currency = null)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(order ?? Order.OrderByValue, currency);
            }
            return await GetOrdererTransactionsAsync(t => t is TransactionDto && t.Depletion == IsExpense, order ?? Order.OrderByValue, currency);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByDateAsync(DateOnly date, bool? IsExpense = null, Order order = Order.OrderByDate, CurrencyDto? currency = null) // If you want to get all regardless of whether it's an expense or income, leave 'IsExpense' as null
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Date == date, order, currency);
            }

            return await GetOrdererTransactionsAsync(t => t.Date == date && t.Depletion == IsExpense, order, currency);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByMonthAsync(int month, int year, bool? IsExpense = null, Order order = Order.OrderByDate, CurrencyDto? currency = null)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Date.Month == month && t.Date.Year == year, order, currency);
            }

            return await GetOrdererTransactionsAsync(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == IsExpense, order, currency);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByYearAsync(int year, bool? IsExpense = null, Order order = Order.OrderByDate, CurrencyDto? currency = null)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Date.Year == year, order, currency);
            }

            return await GetOrdererTransactionsAsync(t => t.Date.Year == year && t.Depletion == IsExpense, order, currency);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByCategoryAsync(string category, bool? IsExpense = null, Order order = Order.OrderByDate, CurrencyDto? currency = null)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactionsAsync(t => t.Category == category, order, currency);
            }

            return await GetOrdererTransactionsAsync(t => t.Category == category && t.Depletion == IsExpense, order, currency);
        }
        public async Task<OperationResult<IEnumerable<TransactionDto>>> GetAllByPredicateAsync(Expression<Func<TransactionDto, bool>> predicate, Order order = Order.OrderByDate, CurrencyDto? currency = null)
        {
            return await GetOrdererTransactionsAsync(predicate, order, currency);
        }
        #endregion
    }
}
