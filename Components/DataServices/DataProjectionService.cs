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
        private static async Task<decimal> GetSummedTransactions(List<TransactionDto> transactions)
        {
            decimal result = 0;

            foreach (var item in transactions)
            {
                result += item.Value;
            }

            return result;
        }


        //public decimal GlobalNet { get { return Income() - Expenses(); } } 
        //public bool GlobalDeficit { get { return GlobalNet < 0; } }

    
        // All transaction data projection

        public async Task<List<TransactionDto>> GetAll()
        {
            return await _StateStorage.GetAllAsync();
        }
        public async Task<List<TransactionDto>> GetAllByDate(DateOnly date, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(t => t.Date == date, order);
        }
        public async Task<List<TransactionDto>> GetAllByMonth(int month, int year, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(t => t.Date.Month == month && t.Date.Year == year, order);
        }
        public async Task<List<TransactionDto>> GetAllByYear(int year, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(t => t.Date.Year == year, order);
        }
        public async Task<List<TransactionDto>> GetAllByCategory(string category, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(t => t.Category == category, order);
        }
        public async Task<List<TransactionDto>> GetAllByPredicate(Expression<Func<TransactionDto, bool>> predicate, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(predicate, order);
        }

        // Expenses data projection

        public async Task<List<TransactionDto>> GetTransaction(bool IsExpense, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(t => t.Depletion == IsExpense, order);
        }
        public async Task<List<TransactionDto>> GetTransactionByDate(bool IsExpense, DateOnly date, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(t => t.Depletion == IsExpense && t.Date == date, order);
        }
        public async Task<List<TransactionDto>> GetTransactionByMonth(bool IsExpense, int month, int year, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(t => t.Depletion == IsExpense && t.Date.Month == month && t.Date.Year == year, order);
        }
        public async Task<List<TransactionDto>> GetTransactionByYear(bool IsExpense, int year, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(t => t.Depletion == IsExpense && t.Date.Year == year, order);
        }
        public async Task<List<TransactionDto>> GetTransactionByCategory(bool IsExpense, string category, Order order = Order.OrderByDate)
        {
            return await GetOrdererTransactions(t => t.Depletion == IsExpense && t.Category == category, order);
        }

        // General financial results
        public async Task<decimal> GetGlobalNet()
        {
            decimal expenses = await GetTotalAmmount(true);
            decimal income = await GetTotalAmmount(false);

            return income - expenses;
        }
        public async Task<bool> GetGlobalDeficit()
        {
            return await GetGlobalNet() > 0;
        }


        public async Task<decimal> GetTotalAmmount(bool IsExpense)
        {
            var transactions = await GetTransaction(IsExpense);
            return await GetSummedTransactions(transactions);
        }
        public async Task<decimal> GetTotalAmmountByPredicate(Expression<Func<TransactionDto, bool>> predicate)
        {
            var transactions = await GetAllByPredicate(predicate);
            return await GetSummedTransactions(transactions);
        }
    }
}
