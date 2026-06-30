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
                return OperationResult.FaultedOperation("An error occurred while trying to connect to the storage system. Please try again");
            }
            catch (DbUpdateException)
            {
                return OperationResult.FaultedOperation("An error occurred while trying to save the changes. Please try again");
            }
            catch (TimeoutException)
            {
                return OperationResult.FaultedOperation("The operation took too long. Please try again");
            }
        }

        public async Task<OperationResult> DeleteAsync(int Id)
        {
            using var context = new StateStorageDbContext(Options);

            var Entity = await context.Set<T>().Where(e => e.Id == Id).FirstOrDefaultAsync();

            if (Entity is null)
            {
                return OperationResult.FaultedOperation("Unexistent entity");
            }

            try
            {
                context.Set<T>().Remove(Entity);
                await context.SaveChangesAsync();

                return OperationResult.SuccessfulOperation();
            }
            catch (SqliteException)
            {
                return OperationResult.FaultedOperation("An error occurred while trying to connect to the storage system. Please try again");
            }
            catch (DbUpdateException)
            {
                return OperationResult.FaultedOperation("An error occurred while trying to save the changes. Please try again");
            }
            catch (TimeoutException)
            {
                return OperationResult.FaultedOperation("The operation took too long. Please try again");
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
                return OperationResult<int>.FaultedOperation("An error occurred while trying to connect to the storage system. Please try again");
            }
            catch (DbUpdateException)
            {
                return OperationResult<int>.FaultedOperation("An error occurred while trying to save the changes. Please try again");
            }
            catch (TimeoutException)
            {
                return OperationResult<int>.FaultedOperation("The operation took too long. Please try again");
            }
        }

        public async Task<OperationResult<List<T>>> GetAllAsync()
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                return OperationResult<List<T>>.SuccessfulOperation(await context.Set<T>().AsNoTracking().ToListAsync());
            }
            catch (SqliteException)
            {
                return OperationResult<List<T>>.FaultedOperation("An error occurred while trying to connect to the storage system. Please try again");
            }
            catch (DbUpdateException)
            {
                return OperationResult<List<T>>.FaultedOperation("An error occurred while trying to save the changes. Please try again");
            }
            catch (TimeoutException)
            {
                return OperationResult<List<T>>.FaultedOperation("The operation took too long. Please try again");
            }
        }

        public async Task<OperationResult<List<T>>> GetEntitiesAsync(Expression<Func<T, bool>> Predicate)
        {
            using var context = new StateStorageDbContext(Options);

            try
            {
                return OperationResult<List<T>>.SuccessfulOperation(await context.Set<T>().AsNoTracking().Where(Predicate).ToListAsync());
            }
            catch (SqliteException)
            {
                return OperationResult<List<T>>.FaultedOperation("An error occurred while trying to connect to the storage system. Please try again");
            }
            catch (DbUpdateException)
            {
                return OperationResult<List<T>>.FaultedOperation("An error occurred while trying to save the changes. Please try again");
            }
            catch (TimeoutException)
            {
                return OperationResult<List<T>>.FaultedOperation("The operation took too long. Please try again");
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
                return OperationResult.FaultedOperation("Entity can't be null");
            }

            if (await context.Set<T>().AnyAsync(e => e.Id == Entity.Id))
            {
                return OperationResult.FaultedOperation("There's alredy an entity with the same Id");
            }

            try
            {
                await context.Set<T>().AddAsync(Entity);
                await context.SaveChangesAsync();

                return OperationResult.SuccessfulOperation();
            }
            catch (SqliteException)
            {
                return OperationResult.FaultedOperation("An error occurred while trying to connect to the storage system. Please try again");
            }
            catch (DbUpdateException)
            {
                return OperationResult.FaultedOperation("An error occurred while trying to save the changes. Please try again");
            }
            catch (TimeoutException)
            {
                return OperationResult.FaultedOperation("The operation took too long. Please try again");
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
                        return OperationResult<int>.FaultedOperation("Entity can't be null");
                    }

                    if (await context.Set<T>().AnyAsync(e => e.Id == entity.Id))
                    {
                        return OperationResult<int>.FaultedOperation("There's alredy an entity with the same Id");
                    }

                    await context.Set<T>().AddAsync(entity);
                }

                var affectedRows = await context.SaveChangesAsync();

                return OperationResult<int>.SuccessfulOperation(affectedRows);
            }
            catch (SqliteException)
            {
                return OperationResult<int>.FaultedOperation("An error occurred while trying to connect to the storage system. Please try again");
            }
            catch (DbUpdateException)
            {
                return OperationResult<int>.FaultedOperation("An error occurred while trying to save the changes. Please try again");
            }
            catch (TimeoutException)
            {
                return OperationResult<int>.FaultedOperation("The operation took too long. Please try again");
            }

        }

        public async Task<OperationResult> UpdateAsync(T NewEntity)
        {
            using var context = new StateStorageDbContext(Options);

            if (!await context.Set<T>().AnyAsync(e => e.Id == NewEntity.Id))
            {
                return OperationResult.FaultedOperation("Unexistent entity");
            }

            try
            {
                context.Set<T>().Update(NewEntity);

                int x = await context.SaveChangesAsync();

                return OperationResult.SuccessfulOperation();
            }
            catch (SqliteException)
            {
                return OperationResult.FaultedOperation("An error occurred while trying to connect to the storage system. Please try again");
            }
            catch (DbUpdateException)
            {
                return OperationResult.FaultedOperation("An error occurred while trying to save the changes. Please try again");
            }
            catch (TimeoutException)
            {
                return OperationResult.FaultedOperation("The operation took too long. Please try again");
            }
        }

        public async Task<OperationResult<int>> UpdateRange(T[] Entities)
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
                return OperationResult<int>.FaultedOperation("An error occurred while trying to connect to the storage system. Please try again");
            }
            catch (DbUpdateException)
            {
                return OperationResult<int>.FaultedOperation("An error occurred while trying to save the changes. Please try again");
            }
            catch (TimeoutException)
            {
                return OperationResult<int>.FaultedOperation("The operation took too long. Please try again");
            }
        }
    }
}
