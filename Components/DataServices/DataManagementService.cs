using Microsoft.EntityFrameworkCore.ChangeTracking;
using Repositories;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DataServices
{
    public class DataManagementService(IStateStorage StateStorage)
    {
        private IStateStorage _StateStorage = StateStorage;

        public async Task<bool> UpdateTransactionAsync(int TransactionId, decimal? value = null, DateOnly? date = null, string? category = null, bool? depletion = null)
        {
            try
            {
                var OldTransaction = await _StateStorage.GetTransactionAsync(TransactionId);
                var NewTransaction = new TransactionDto() { TransactionId = TransactionId, Value = value ?? OldTransaction!.Value, Date = date ?? OldTransaction!.Date, Category = category ?? OldTransaction!.Category, Depletion = depletion ?? OldTransaction!.Depletion };

                return await _StateStorage.UpdateAsync(TransactionId, NewTransaction);
            }
            catch (Exception)
            {
                return false;
            }
        }
       
       

        public async Task<bool> RemoveTransactionAsync(int TransactionId)
        {
            try
            {
                return await _StateStorage.DeleteAsync(TransactionId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveFixedTransaction(int CollectionId, int FromDuration)
        {
            try
            {
                return await _StateStorage.DeleteFromRangeAsync(t => t is FixedTransactionDto && (t as FixedTransactionDto)!.FixedTransactionId == CollectionId && (t as FixedTransactionDto)!.Duration < FromDuration);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> RemoveFixedTransaction(int CollectionId)
        {
            try
            {
                return await _StateStorage.DeleteFromRangeAsync(t => t is FixedTransactionDto && (t as FixedTransactionDto)!.FixedTransactionId == CollectionId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveExpenses()
        {
            try
            {
                return await _StateStorage.DeleteFromRangeAsync(t => t.Depletion == true);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveIncome()
        {
            try
            {
                return await _StateStorage.DeleteFromRangeAsync(t => t.Depletion == false);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromCategory(string category)
        {
            try
            {
                return await _StateStorage.DeleteFromRangeAsync(t => t.Category == category);
            }
            catch (Exception)
            { 
                return false;
            }
        }

        public async Task<bool> RestartData()
        {
            try
            {
                return await _StateStorage.ClearStorageAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
