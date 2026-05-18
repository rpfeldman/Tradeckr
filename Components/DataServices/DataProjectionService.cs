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
    public sealed class DataProjectionService(IStateStorage StateStorage)
    {
        private IStateStorage _StateStorage = StateStorage;
        public enum Order
        {
            OrderByDate, OrderByValue
        }
        private async Task<List<TransactionDto>> GetOrdererTransactions(Expression<Func<TransactionDto, bool>> predicate, Order order)
        {
            var transactions = await _StateStorage.GetTransactionsAsync(predicate);
            switch (order)
            {
                case Order.OrderByDate:
                    return [.. transactions.OrderBy(t => t.Date)];

                case Order.OrderByValue:
                    return [.. transactions.OrderBy(t => t.Value)];

                default: return [];
            }
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

        #region All transactions data projection
        public async Task<List<TransactionDto>> GetAll(bool? IsExpense = null, Order ? order = null)
        {
            List<TransactionDto> Transactions = new();

            if(IsExpense is null)
            {
                Transactions = await _StateStorage.GetAllAsync();
            }
            else
            {
                Transactions = await _StateStorage.GetAllAsync();
                Transactions = [.. Transactions.Where(t => t.Depletion == IsExpense)];
            }
            

            switch (order)
            {
                case Order.OrderByDate:
                    return [.. Transactions.OrderBy(t => t.Date)];

                case Order.OrderByValue:
                    return [.. Transactions.OrderBy(t => t.Value)];


                default: return Transactions;
            }
        }
        public async Task<List<TransactionDto>> GetAllByDate(DateOnly date, bool? IsExpense = null, Order order = Order.OrderByDate) // If you want to get all regardless of whether it's an expense or income, leave 'IsExpense' as null
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactions(t => t.Date == date, order);
            }

            return await GetOrdererTransactions(t => t.Date == date && t.Depletion == IsExpense, order);
        }
        public async Task<List<TransactionDto>> GetAllByMonth(int month, int year, bool? IsExpense = null, Order order = Order.OrderByDate)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactions(t => t.Date.Month == month && t.Date.Year == year, order);
            }

            return await GetOrdererTransactions(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == IsExpense, order);
        }
        public async Task<List<TransactionDto>> GetAllByYear(int year, bool? IsExpense = null, Order order = Order.OrderByDate)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactions(t => t.Date.Year == year, order);
            }

            return await GetOrdererTransactions(t => t.Date.Year == year && t.Depletion == IsExpense, order);
        }
        public async Task<List<TransactionDto>> GetAllByCategory(string category, bool? IsExpense = null, Order order = Order.OrderByDate)
        {
            if(IsExpense is null)
            {
                return await GetOrdererTransactions(t => t.Category == category, order);
            }

            return await GetOrdererTransactions(t => t.Category == category && t.Depletion == IsExpense, order);
        }
        public async Task<List<TransactionDto>> GetAllByPredicate(Expression<Func<TransactionDto, bool>> predicate, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(predicate, order);
        }
        #endregion
        #region General financial results
        public async Task<decimal> GetGlobalResult()
        {
            var expenses = GetAll(true);
            var income = GetAll(false);

            var results = await Task.WhenAll(income, expenses);

            return GetSummedTransactions(results[0]) - GetSummedTransactions(results[1]);
        }
        public async Task<decimal> GetResultsByDate(DateOnly date)
        {
            var expenses = GetAllByDate(date, true);
            var income = GetAllByDate(date, false);

            var results = await Task.WhenAll(income, expenses);

            return GetSummedTransactions(results[0]) - GetSummedTransactions(results[1]);
        }
        public async Task<decimal> GetResultsByMonth(int month, int year)
        {
            var expenses = GetAllByMonth(month, year, true);
            var income = GetAllByMonth(month, year, false);

            var results = await Task.WhenAll(income, expenses);

            return GetSummedTransactions(results[0]) - GetSummedTransactions(results[1]);
        }
        public async Task<decimal> GetResultsByYear(int year)
        {
            var expenses = GetAllByYear(year, true);
            var income = GetAllByYear(year, false);

            var results = await Task.WhenAll(income, expenses);

            return GetSummedTransactions(results[0]) - GetSummedTransactions(results[1]);
        }
        public async Task<decimal> GetResultsByCategory(string category)
        {
            var expenses = GetAllByCategory(category, true);
            var income = GetAllByCategory(category, false);

            var results = await Task.WhenAll(income, expenses);

            return GetSummedTransactions(results[0]) - GetSummedTransactions(results[1]);
        }
        public async Task<decimal> GetResultsByPredicate(Expression<Func<TransactionDto, bool>> predicate)
        {
            var transactions = await GetAllByPredicate(predicate);
            return GetSummedTransactions(transactions);
        }
        #endregion
    }
}
