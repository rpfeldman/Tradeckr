using DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Repositories
{
    public sealed class EF_SQLite_StateStorage : IStateStorage
    {
        private StateStorageDbContext Context;
        public EF_SQLite_StateStorage(string StorageFilePath, int[] DecimalValuePrecision)
        {
            var options = new DbContextOptionsBuilder().UseSqlite($"Data source={StorageFilePath}").Options;
            Context = new(options, DecimalValuePrecision); // As SQLite save decimal data type as TEXT, 'DecimalValuePrecision' it's irrelevant in this case
            Context.Database.EnsureCreated();
        }

        /// <summary>
        /// Generates and ID for fixed transactions collections
        /// </summary>
        /// <returns></returns>
        private int IdSetterForFixedTransactions() // As EF core can't generate values for non-keys properties and multiple instances will have the same ID, I decided make this method
        {
            FixedTransactionDto? Last = (FixedTransactionDto?)Context.TransactionsTable.Where(t => t is FixedTransactionDto).OrderBy(t => (t as FixedTransactionDto)!.FixedTransactionId).LastOrDefault();

            return Last != null ? (Last.FixedTransactionId + 1) : 1;
        }

        public void ClearStorage()
        {
            Context.TransactionsTable.RemoveRange(Context.TransactionsTable);
            Context.SaveChanges();
        }

        public List<TransactionDto> GetAll()
        {
            return Context.TransactionsTable.ToList();
        }

        public TransactionDto? GetTransaction(int TransactionId)
        {
            return Context.TransactionsTable.Where(t => t.TransactionId == TransactionId).FirstOrDefault();
        }

        public List<TransactionDto> GetTransactions(Expression<Func<TransactionDto, bool>> predicate)
        {
            return Context.TransactionsTable.Where(predicate).ToList();
        }

        public void Delete(int TransactionId)
        {
            var Transaction = GetTransaction(TransactionId) ?? throw new Exception("Unexistent transaction");

            Context.TransactionsTable.Remove(Transaction);
            Context.SaveChanges();
        }

        public void DeleteFromRange(Expression<Func<TransactionDto, bool>> predicate)
        {
            Context.RemoveRange(GetTransactions(predicate));
            Context.SaveChanges();
        }

        public void Save(decimal value, DateOnly date, string category, bool depletion, bool isfixed = false, int? duration = null)
        {
            TransactionDto Transaction;

            if (isfixed && duration != null)
            {
                int CollectionId = IdSetterForFixedTransactions();
             
                for (int i = 0; i < duration; i++)
                {
                    Transaction = new FixedTransactionDto() { Value = value, Date = date, Category = category, Depletion = depletion, Fixed = isfixed, Duration = ((int)duration - i), FixedTransactionId = CollectionId };
                    date = date.AddMonths(1);

                    Context.Add(Transaction);
                }
                Context.SaveChanges();
                return;
            }

            Transaction = new TransactionDto() { Value = value, Date = date, Category = category, Depletion = depletion, Fixed = isfixed };

            Context.Add(Transaction);
            Context.SaveChanges();
        }

        public void Update(int TransactionId, decimal? value=null, DateOnly? date=null, string? category=null, bool? depletion=null)
        {
            var Transaction = GetTransaction(TransactionId) ?? throw new Exception("Unexistent transaction");

            Transaction.Value = value ?? Transaction.Value;
            Transaction.Date = date ?? Transaction.Date;
            Transaction.Category = category ?? Transaction.Category;
            Transaction.Depletion = depletion ?? Transaction.Depletion;

            Context.Update(Transaction);
            Context.SaveChanges();
        }
    }
}
