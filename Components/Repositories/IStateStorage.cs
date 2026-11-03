using DomainModel;
using Microsoft.EntityFrameworkCore.Query;
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
        public Task<OperationResult<int>> UpdateRangeAsync(Action<UpdateSettersBuilder<TEntity>> setter);
        public Task<OperationResult<int>> UpdateRangeAsync(Expression<Func<TEntity, bool>> filter, Action<UpdateSettersBuilder<TEntity>> setter);
        public Task<OperationResult<bool>> AnyAsync();
        public Task<OperationResult<bool>> AnyAsync(Expression<Func<TEntity, bool>> Predicate);
        public Task<OperationResult> ClearStorageAsync();
        public Task<Option<TEntity>> GetEntityAsync(int Id);
        public Task<OperationResult<IEnumerable<TEntity>>> GetEntitiesAsync(Expression<Func<TEntity, bool>> Predicate);
        public Task<OperationResult<IEnumerable<TEntity>>> GetAllAsync();
        public Task<OperationResult<IEnumerable<TResult>>> ProjectAsync<TResult>(Expression<Func<TEntity, TResult>> Selector);
        public Task<OperationResult<IEnumerable<TResult>>> ProjectByPredicateAsync<TResult>(Expression<Func<TEntity, TResult>> Selector, Expression<Func<TEntity, bool>> Predicate);
    }
}
