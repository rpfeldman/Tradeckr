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

        public async Task<OperationResult> DeleteFromRangeAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                _Table.RemoveRange(await _Table.Where(predicate).ToArrayAsync());
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

        public async Task<OperationResult<List<T>>> GetEntitiesAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return OperationResult<List<T>>.SuccessfulOperation(await _Table.AsNoTracking().Where(predicate).ToListAsync());
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
            var id = Entity.Id;

            if (await _Table.AnyAsync(e => e.Id == id))
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

        public async Task<OperationResult> UpdateAsync(T NewEntity)
        {
            if (!await _Table.AnyAsync(e => e.Id == NewEntity.Id))
            {
                return OperationResult.FaultedOperation("Unexistent entity");
            }

            try
            {
                _Table.Update(NewEntity);

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

        public async Task<OperationResult> UpdateRange(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
