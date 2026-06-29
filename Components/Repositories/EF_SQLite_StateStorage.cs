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
        private StateStorageDbContext Context;
        private readonly DbSet<T> _Table;
        public EF_SQLite_StateStorageRepo(string StorageFilePath)
        {
            var options = new DbContextOptionsBuilder().UseSqlite($"Data source={StorageFilePath}").Options;
            Context = new(options, [0, 0]);

            _Table = Context.Set<T>();
            Context.Database.EnsureCreated();

        }
        public async Task<OperationResult> ClearStorageAsync()
        {
            try
            {
                _Table.RemoveRange(_Table);
                await Context.SaveChangesAsync();

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
            var Entity = await _Table.Where(e => e.Id == Id).FirstOrDefaultAsync();

            if (Entity is null)
            {
                return OperationResult.FaultedOperation("Unexistent entity");
            }

            try
            {
                _Table.Remove(Entity);
                await Context.SaveChangesAsync();

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
            try
            {
                _Table.RemoveRange(await _Table.Where(Predicate).ToArrayAsync());
                var affectedRows = await Context.SaveChangesAsync();

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
            try
            {
                return OperationResult<List<T>>.SuccessfulOperation(await _Table.AsNoTracking().ToListAsync());
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
            try
            {
                return OperationResult<List<T>>.SuccessfulOperation(await _Table.AsNoTracking().Where(Predicate).ToListAsync());
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
            var entity = await _Table.AsNoTracking().Where(e => e.Id == Id).FirstOrDefaultAsync();

            if (entity is null)
            {
                return Option<T>.None();
            }

            return Option<T>.Some(entity);
        }

        public async Task<OperationResult> SaveAsync(T Entity)
        {
            if(Entity is null)
            {
                return OperationResult.FaultedOperation("Entity can't be null");
            }

            if (await _Table.AnyAsync(e => e.Id == Entity.Id))
            {
                return OperationResult.FaultedOperation("There's alredy an entity with the same Id");
            }

            try
            {
                await _Table.AddAsync(Entity);
                await Context.SaveChangesAsync();

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
            try
            {
                foreach (var entity in Entities)
                {
                    if (entity is null)
                    {
                        return OperationResult<int>.FaultedOperation("Entity can't be null");
                    }

                    if (await _Table.AnyAsync(e => e.Id == entity.Id))
                    {
                        return OperationResult<int>.FaultedOperation("There's alredy an entity with the same Id");
                    }

                    await _Table.AddAsync(entity);
                }

                var affectedRows = await Context.SaveChangesAsync();

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
            if (!await _Table.AnyAsync(e => e.Id == NewEntity.Id))
            {
                return OperationResult.FaultedOperation("Unexistent entity");
            }

            try
            {
                _Table.Update(NewEntity);

                int x = await Context.SaveChangesAsync();

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
            try
            {
                _Table.UpdateRange(Entities);
                var affectedRows = await Context.SaveChangesAsync();

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
