using DomainModel;
using Repositories;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public decimal GlobalNet { get { return Income() - Expenses(); } } 
        public bool GlobalDeficit { get { return GlobalNet < 0; } }

        // A lot of lines of code but extremely easy usage of this service

        // All transaction data projection
        public List<TransactionDto> GetAll(Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetAll().OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetAll().OrderBy(t => t.Value).ToList();

                default:
                    return _StateStorage.GetAll();
            }
        }
        public List<TransactionDto> GetAllByDate(DateOnly date)
        {
            return _StateStorage.GetTransactions(t => t.Date == date);
        }
        public List<TransactionDto> GetAllByMonth(int month, int year, Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Date.Month == month && t.Date.Year == year).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Date.Month == month && t.Date.Year == year).OrderBy(t => t.Value).ToList();

                default:
                    return new List<TransactionDto>();
            }
        }
        public List<TransactionDto> GetAllByYear(int year, Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Date.Year == year).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Date.Year == year).OrderBy(t => t.Value).ToList();

                default: return _StateStorage.GetTransactions(t => t.Date.Year == year);
            }
        }

        // Expenses data projection
        public List<TransactionDto> GetExpenses(Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Depletion == true).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Depletion == true).OrderBy(t => t.Value).ToList();

                default: return _StateStorage.GetTransactions(t => t.Depletion == true);
            }
        }
        public List<TransactionDto> GetExpensesByCategory(string category, Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Category == category && t.Depletion == true).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Category == category && t.Depletion == true).OrderBy(t => t.Value).ToList();

                default: return _StateStorage.GetTransactions(t => t.Category == category && t.Depletion == true);
            }
        }
        public List<TransactionDto> GetExpensesByDate(DateOnly date)
        {
            return _StateStorage.GetTransactions(t => t.Date == date && t.Depletion == true);
        }
        public List<TransactionDto> GetExpensesByMonth(int month, int year, Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == true).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == true).OrderBy(t => t.Value).ToList();

                default:
                    return _StateStorage.GetTransactions(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == true);
            }
        }
        public List<TransactionDto> GetExpensesByYear(int year, Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Date.Year == year && t.Depletion == true).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Date.Year == year && t.Depletion == true).OrderBy(t => t.Value).ToList();

                default: return _StateStorage.GetTransactions(t => t.Date.Year == year && t.Depletion == true);
            }
        }

        // Income data projection
        public List<TransactionDto> GetIncome(Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Depletion == false).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Depletion == false).OrderBy(t => t.Value).ToList();

                default: return _StateStorage.GetTransactions(t => t.Depletion == false);
            }
        }
        public List<TransactionDto> GetIncomeByCategory(string category, Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Depletion == false && t.Category == category).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Depletion == false && t.Category == category).OrderBy(t => t.Value).ToList();

                default: return _StateStorage.GetTransactions(t => t.Depletion == false && t.Category == category);
            }
        }
        public List<TransactionDto> GetIncomeByDate(DateOnly date, Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Depletion == false && t.Date == date).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Depletion == false && t.Date == date).OrderBy(t => t.Value).ToList();

                default: return _StateStorage.GetTransactions(t => t.Depletion == false && t.Date == date);
            }
        }
        public List<TransactionDto> GetIncomeByMonth(int month, int year, Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == false).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == false).OrderBy(t => t.Value).ToList();

                default: return _StateStorage.GetTransactions(t => t.Date.Month == month && t.Date.Year == year && t.Depletion == false);
            }
        }
        public List<TransactionDto> GetIncomeByYear(int year, Order order = Order.OrderByDate)
        {
            switch (order)
            {
                case Order.OrderByDate:
                    return _StateStorage.GetTransactions(t => t.Depletion == false && t.Date.Year == year).OrderBy(t => t.Date).ToList();

                case Order.OrderByValue:
                    return _StateStorage.GetTransactions(t => t.Depletion == false && t.Date.Year == year).OrderBy(t => t.Value).ToList();

                default: return _StateStorage.GetTransactions(t => t.Depletion == false && t.Date.Year == year);
            }
        }

        // Expenses financial results
        public decimal Expenses()
        {
            decimal expenses = 0m;

            foreach (var item in GetExpenses())
            {
                expenses += item.Value;
            }

            return expenses;
        }
        public decimal ExpensesByCategory(string category)
        {
            decimal expenses = 0m;

            foreach (var item in GetExpensesByCategory(category))
            {
                expenses += item.Value;
            }

            return expenses;
        }
        public decimal ExpensesByDate(DateOnly date)
        {
            decimal expenses = 0m;

            foreach (var item in GetExpensesByDate(date))
            {
                expenses += item.Value;
            }

            return expenses;
        }
        public decimal ExpensesByMonth(int month, int year)
        {
            decimal expenses = 0m;

            foreach (var item in GetExpensesByMonth(month, year))
            {
                expenses += item.Value;
            }

            return expenses;
        }
        public decimal ExpensesByYear(int year)
        {
            decimal expenses = 0m;

            foreach (var item in GetExpensesByYear(year))
            {
                expenses += item.Value;
            }

            return expenses;
        }

        // Income financial results
        public decimal Income()
        {
            decimal income = 0m;

            foreach (var item in GetIncome())
            {
                income += item.Value;
            }

            return income;
        }
        public decimal IncomeByCategory(string category)
        {
            decimal income = 0m;

            foreach (var item in GetIncomeByCategory(category))
            {
                income += item.Value;
            }

            return income;
        }
        public decimal IncomeByDate(DateOnly date)
        {
            decimal income = 0m;

            foreach (var item in GetIncomeByDate(date))
            {
                income += item.Value;
            }

            return income;
        }
        public decimal IncomeByMonth(int month, int year)
        {
            decimal income = 0m;

            foreach (var item in GetIncomeByMonth(month, year))
            {
                income += item.Value;
            }

            return income;
        }
        public decimal IncomeByYear(int year)
        {
            decimal income = 0m;

            foreach (var item in GetIncomeByYear(year))
            {
                income += item.Value;
            }

            return income;
        }

        // General financial results
        public decimal NetByCategory(string category) 
        {
            return IncomeByCategory(category) - ExpensesByCategory(category);
        }
        public bool DeficitByCategory(string category)
        {
            return NetByCategory(category) < 0;
        }
        public decimal NetByDate(DateOnly date)
        {
            return IncomeByDate(date) - ExpensesByDate(date);
        }
        public bool DeficitByDate(DateOnly date)
        {
            return NetByDate(date) < 0;
        }
        public decimal NetByMonth(int month, int year)
        {
            return IncomeByMonth(month, year) - ExpensesByMonth(month, year);
        }
        public bool DeficitByMonth(int month, int year)
        {
            return NetByMonth(month, year) < 0;
        }
        public decimal NetByYear(int year)
        {
            return IncomeByYear(year) - ExpensesByYear(year);
        }
        public bool DeficitByYear(int year)
        {
            return NetByYear(year) < 0;
        }
    }
}
