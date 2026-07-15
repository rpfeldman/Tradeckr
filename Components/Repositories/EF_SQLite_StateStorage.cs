using DomainModel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Repositories
{
    public sealed class EF_SQLite_StateStorageRepo<T> : IStateStorage<T> where T : class, IEntity
    {
        // As SQLite save decimal data type as TEXT, 'DecimalValuePrecision' it's irrelevant in this case
        private StateStorageDbContext Context;
        private DbContextOptions Options;
        public EF_SQLite_StateStorageRepo(string StorageFilePath)
        {
            Options = new DbContextOptionsBuilder().UseSqlite($"Data source={StorageFilePath}").Options; 
            Context = new(Options); 

            Context.Database.EnsureCreated();
        }
        public async Task<OperationResult> ClearStorageAsync()
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                context.Set<T>().RemoveRange(context.Set<T>());
                await context.SaveChangesAsync();

                return OperationResult.SuccessfulOperation();
            }
            catch (SqliteException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<OperationResult> DeleteAsync(int Id)
        {
            using var context = new StateStorageDbContext(Options);

            var Entity = await context.Set<T>().Where(e => e.Id == Id).FirstOrDefaultAsync();

            if (Entity is null)
            {
                return OperationResult.FaultedOperation(new InnerErrorDto() { ErrorMessage = $"Unexistent {typeof(T).Name}" });
            }

            try
            {
                context.Set<T>().Remove(Entity);
                await context.SaveChangesAsync();

                return OperationResult.SuccessfulOperation();
            }
            catch (SqliteException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<OperationResult<int>> DeleteFromRangeAsync(Expression<Func<T, bool>> Predicate)
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                context.Set<T>().RemoveRange(await context.Set<T>().Where(Predicate).ToArrayAsync());
                var affectedRows = await context.SaveChangesAsync();

                return OperationResult<int>.SuccessfulOperation(affectedRows);
            }
            catch (SqliteException)
            {
                return OperationResult<int>.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult<int>.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult<int>.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<OperationResult> DeleteRangeAsync(T[] Entities)
        { 
            using var context = new StateStorageDbContext(Options);

            try
            {
                if(Entities.Any(e => e is null))
                {
                    return OperationResult.FaultedOperation(new InnerErrorDto() { ErrorMessage = $"{typeof(T).Name} can't be null" });
                }

                Context.Set<T>().RemoveRange(Entities);

                var a = await Context.SaveChangesAsync();

                return OperationResult.SuccessfulOperation();
            }
            catch (SqliteException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<OperationResult<IEnumerable<T>>> GetAllAsync()
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                var result = await context.Set<T>().AsNoTracking().ToListAsync(); // I wanted to use 'IAsyncEnumerable' to create a 1-to-1 data stream. But since we're using SQLite with a small amount of data, I felt it was a bit over-engineering.
                return OperationResult<IEnumerable<T>>.SuccessfulOperation(result);
            }
            catch (SqliteException)
            {
                return OperationResult<IEnumerable<T>>.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult<IEnumerable<T>>.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult<IEnumerable<T>>.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<OperationResult<IEnumerable<T>>> GetEntitiesAsync(Expression<Func<T, bool>> Predicate)
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                var result = await context.Set<T>().AsNoTracking().Where(Predicate).ToListAsync();

                return OperationResult<IEnumerable<T>>.SuccessfulOperation(result);
            }
            catch (SqliteException)
            {
                return OperationResult<IEnumerable<T>>.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult<IEnumerable<T>>.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult<IEnumerable<T>>.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<Option<T>> GetEntityAsync(int Id)
        {
            using var context = new StateStorageDbContext(Options);

            var entity = await context.Set<T>().AsNoTracking().Where(e => e.Id == Id).FirstOrDefaultAsync();

            if (entity is null)
            {
                return Option<T>.None();
            }

            return Option<T>.Some(entity);
        }

        public async Task<OperationResult> SaveAsync(T Entity)
        {
            using var context = new StateStorageDbContext(Options);

            if (Entity is null)
            {
                return OperationResult.FaultedOperation(new InnerErrorDto() { ErrorMessage = $"{typeof(T).Name} can't be null" });
            }

            if (await context.Set<T>().AnyAsync(e => e.Id == Entity.Id))
            {
                return OperationResult.FaultedOperation(new InnerErrorDto() { ErrorMessage = $"Another {typeof(T).Name} with the same Id was found" });
            }

            try
            {
                await context.Set<T>().AddAsync(Entity);
                await context.SaveChangesAsync();

                return OperationResult.SuccessfulOperation();
            }
            catch (SqliteException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<OperationResult<int>> SaveRangeAsync(T[] Entities)
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                foreach (var entity in Entities)
                {
                    if (entity is null)
                    {
                        return OperationResult<int>.FaultedOperation(new InnerErrorDto() { ErrorMessage = $"{typeof(T).Name} can't be null" });
                    }

                    if (await context.Set<T>().AnyAsync(e => e.Id == entity.Id))
                    {
                        return OperationResult<int>.FaultedOperation(new InnerErrorDto() { ErrorMessage = $"Another {typeof(T).Name} with the same Id was found" });
                    }

                    await context.Set<T>().AddAsync(entity);
                }

                var affectedRows = await context.SaveChangesAsync();

                return OperationResult<int>.SuccessfulOperation(affectedRows);
            }
            catch (SqliteException)
            {
                return OperationResult<int>.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult<int>.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult<int>.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<OperationResult> UpdateAsync(T NewEntity)
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                context.Set<T>().Update(NewEntity);

                int x = await context.SaveChangesAsync();
                if(x == 1)
                {
                    return OperationResult.SuccessfulOperation();
                }

                return OperationResult.FaultedOperation(new InnerErrorDto() { ErrorMessage = $"No records were updated. Please verify that the entity with ID {NewEntity.Id} exists" });
            }
            catch (SqliteException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<OperationResult<int>> UpdateRangeAsync(T[] Entities)
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                context.Set<T>().UpdateRange(Entities);
                var affectedRows = await context.SaveChangesAsync();

                return OperationResult<int>.SuccessfulOperation(affectedRows);
            }
            catch (SqliteException)
            {
                return OperationResult<int>.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult<int>.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult<int>.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }

        public async Task<OperationResult<bool>> AnyAsync()
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                var anyOperation = await context.Set<T>().AnyAsync();

                return OperationResult<bool>.SuccessfulOperation(anyOperation);
            }
            catch (SqliteException)
            {
                return OperationResult<bool>.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult<bool>.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult<bool>.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }
        public async Task<OperationResult<bool>> AnyAsync(Expression<Func<T, bool>> Predicate)
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                var anyOperation = await context.Set<T>().AnyAsync(Predicate);

                return OperationResult<bool>.SuccessfulOperation(anyOperation);
            }
            catch (SqliteException)
            {
                return OperationResult<bool>.FaultedOperation(RepositorieErrors.DBProviderError);
            }
            catch (DbUpdateException)
            {
                return OperationResult<bool>.FaultedOperation(RepositorieErrors.DBUpdateError);
            }
            catch (TimeoutException)
            {
                return OperationResult<bool>.FaultedOperation(RepositorieErrors.TimeoutError);
            }
        }
    }
}
