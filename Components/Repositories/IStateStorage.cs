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
        public Task<OperationResult<int>> SaveRangeAsync(TEntity[] Entities);
        public Task<OperationResult> DeleteAsync(int Id);
        public Task<OperationResult<int>> DeleteFromRangeAsync(Expression<Func<TEntity, bool>> Predicate);

        public Task<OperationResult> DeleteRangeAsync(TEntity[] Entities);
        public Task<OperationResult> UpdateAsync(TEntity NewEntity);
        public Task<OperationResult<int>> UpdateRangeAsync(TEntity[] Entities);
        public Task<OperationResult<bool>> AnyAsync();
        public Task<OperationResult<bool>> AnyAsync(Expression<Func<TEntity, bool>> Predicate);
        public Task<OperationResult> ClearStorageAsync();
        public Task<Option<TEntity>> GetEntityAsync(int Id);
        public Task<OperationResult<List<TEntity>>> GetEntitiesAsync(Expression<Func<TEntity, bool>> Predicate);
        public Task<OperationResult<List<TEntity>>> GetAllAsync();
    }
}
