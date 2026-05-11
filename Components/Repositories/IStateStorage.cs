using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Repositories
{
    public interface IStateStorage
    {
        public Task<bool> SaveAsync(decimal value, DateOnly date, string category, bool depletion, bool isfixed = false, int? duration = null);
        public Task<bool> DeleteAsync(int TransactionId);
        public Task<bool> DeleteFromRangeAsync(Expression<Func<TransactionDto, bool>> predicate);
        public Task<bool> UpdateAsync(int TransactionId, TransactionDto NewTransaction);
        public Task<bool> ClearStorageAsync();
        public Task<TransactionDto?> GetTransactionAsync(int TransactionId);
        public Task<List<TransactionDto>> GetTransactionsAsync(Expression<Func<TransactionDto, bool>> predicate);
        public Task<List<TransactionDto>> GetAllAsync();
    }
}
