using DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq.Expressions;

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

        public async Task<bool> ClearStorageAsync()
        {
            Context.TransactionsTable.RemoveRange(Context.TransactionsTable);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<List<TransactionDto>> GetAllAsync()
        {
            return await Context.TransactionsTable.AsNoTracking().ToListAsync();
        }

        public async Task<TransactionDto?> GetTransactionAsync(int TransactionId)
        {
            return await Context.TransactionsTable.AsNoTracking().Where(t => t.TransactionId == TransactionId).FirstOrDefaultAsync();
        }

        public async Task<List<TransactionDto>> GetTransactionsAsync(Expression<Func<TransactionDto, bool>> predicate)
        {
            return await Context.TransactionsTable.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<bool> DeleteAsync(int TransactionId)
        {
            var Transaction = await GetTransactionAsync(TransactionId) ?? throw new Exception("Unexistent transaction");

            Context.TransactionsTable.Remove(Transaction);

            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteFromRangeAsync(Expression<Func<TransactionDto, bool>> predicate)
        {
            Context.RemoveRange(await GetTransactionsAsync(predicate));

            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveAsync(decimal value, DateOnly date, string category, bool depletion, bool isfixed = false, int? duration = null)
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

                return await Context.SaveChangesAsync() > 0;
            }

            Transaction = new TransactionDto() { Value = value, Date = date, Category = category, Depletion = depletion, Fixed = isfixed };

            Context.Add(Transaction);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int TransactionId, TransactionDto NewTransaction)
        {
            var Transaction = await Context.TransactionsTable.Where(t => t.TransactionId == TransactionId).FirstOrDefaultAsync() ?? throw new Exception("Unexistent transaction");

            Transaction.Category = NewTransaction.Category;
            Transaction.Date = NewTransaction.Date;
            Transaction.Depletion = NewTransaction.Depletion;

            return await Context.SaveChangesAsync() > 0;
        }
    }
}
