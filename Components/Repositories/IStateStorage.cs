using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Repositories
{
    public interface IStateStorage<TEntity> where TEntity : IEntity
    {
        public Task<OperationResult> SaveAsync(TEntity Entity);
        public Task<OperationResult> SaveRangeAsync(TEntity[] Entities);
        public Task<OperationResult> DeleteAsync(int Id);
        public Task<OperationResult> DeleteFromRangeAsync(Expression<Func<TEntity, bool>> predicate);
        public Task<OperationResult> UpdateAsync(TEntity NewEntity);
        public Task<OperationResult> UpdateRange(Expression<Func<TEntity, bool>> predicate); // Later on I'll see what to do with this
        public Task<OperationResult> ClearStorageAsync();
        public Task<Option<TEntity>> GetEntityAsync(int Id);
        public Task<OperationResult<List<TEntity>>> GetEntitiesAsync(Expression<Func<TEntity, bool>> predicate);
        public Task<OperationResult<List<TEntity>>> GetAllAsync();
    }
}
