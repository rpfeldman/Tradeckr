using DomainModel;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Repositories;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;

namespace DataServices
{
    public class DataManagementService(IStateStorage<TransactionDto> StateStorage)
    {
        private IStateStorage<TransactionDto> _StateStorage = StateStorage;
        public async Task<OperationResult> UpdateTransactionAsync(int TransactionId, decimal? value = null, DateOnly? date = null, string? category = null, bool? depletion = null)
        {
            if (value < 1m || value > 1000000000m)
            {
                return OperationResult.FaultedOperation(ServiceErrors.ValueOutOfRangeError(1, 1_000_000_000));
            }
            if (category is not null && category.IsWhiteSpace())
            {
                return OperationResult.FaultedOperation(ServiceErrors.EmptyFieldError(nameof(category)));
            }

            var getTransactionOperation = await _StateStorage.GetEntityAsync(TransactionId);

            if (!getTransactionOperation.HasValue)
            {
                return OperationResult.FaultedOperation(ServiceErrors.NotFoundId(TransactionId));
            } 
            TransactionDto Transaction = getTransactionOperation.Value!;

            Transaction.Category = category ?? Transaction.Category;
            Transaction.Value = value ?? Transaction.Value;
            Transaction.Date = date ?? Transaction.Date;
            Transaction.Depletion = depletion ?? Transaction.Depletion;

            return await _StateStorage.UpdateAsync(Transaction);
        }

        public async Task<OperationResult> RenameCategoryAsync(string OldName, string NewName) 
        {
            var anyTransactionOperation = await _StateStorage.AnyAsync(t => t.Category == OldName);

            if (!anyTransactionOperation.Success)
            {
                return OperationResult.FaultedOperation(anyTransactionOperation.InnerError);
            }
            if (!anyTransactionOperation.Result)
            {
                return OperationResult.FaultedOperation(ServiceErrors.NotFoundCategoryNameError(OldName));
            }

            var getOldTransactionsOperation = await _StateStorage.GetEntitiesAsync(t => t.Category == OldName);
            
            if(!getOldTransactionsOperation.Success) { return OperationResult.FaultedOperation(getOldTransactionsOperation.InnerError); }

            var transactions = getOldTransactionsOperation.Result!;

            foreach (var transaction in transactions)
            {
                transaction.Category = NewName;
            }

            var updateRangeOperation = await _StateStorage.UpdateRangeAsync(transactions.ToArray());

            if (updateRangeOperation.Success)
            {
                if (updateRangeOperation.Result != transactions.Count())
                {
                    return OperationResult.FaultedOperation(ServiceErrors.PartialRegistrationError("movements", "renamed"));
                }

                return OperationResult.SuccessfulOperation();
            }

            return OperationResult.FaultedOperation(updateRangeOperation.InnerError);
        }

        public async Task<OperationResult> RemoveTransactionAsync(int TransactionId)
        {
            return await _StateStorage.DeleteAsync(TransactionId);
        }

        public async Task<OperationResult> RemoveFixedTransactionAsync(int CollectionId, int FromDuration)
        {
            var DeleteFromRangeOperation = await _StateStorage.DeleteFromRangeAsync(t => t is FixedTransactionDto && (t as FixedTransactionDto)!.FixedTransactionId == CollectionId && (t as FixedTransactionDto)!.Duration <= FromDuration);

            if (DeleteFromRangeOperation.Success)
            {
                if (DeleteFromRangeOperation.Result < 1) { return OperationResult.FaultedOperation(ServiceErrors.NoFixedTransactionsMatchedError(CollectionId, FromDuration)); };

                return OperationResult.SuccessfulOperation();
            }

            return OperationResult.FaultedOperation(DeleteFromRangeOperation.InnerError);
        }
        public async Task<OperationResult> RemoveFixedTransactionAsync(int CollectionId)
        {
            var DeleteFromRangeOperation = await _StateStorage.DeleteFromRangeAsync(t => t is FixedTransactionDto && (t as FixedTransactionDto)!.FixedTransactionId == CollectionId);

            if (DeleteFromRangeOperation.Success)
            {
                if (DeleteFromRangeOperation.Result < 1) { return OperationResult.FaultedOperation(ServiceErrors.NoCollectionMatchedError("Fixed transaction", CollectionId.ToString())); };
                return OperationResult.SuccessfulOperation();
            }

            return OperationResult.FaultedOperation(DeleteFromRangeOperation.InnerError);
        }

        public async Task<OperationResult> RemoveFromCategoryAsync(string category)
        {
            var DeleteFromRangeOperation = await _StateStorage.DeleteFromRangeAsync(t => t.Category == category);

            if (DeleteFromRangeOperation.Success)
            {
                if (DeleteFromRangeOperation.Result < 1) { return OperationResult.FaultedOperation(ServiceErrors.NoCollectionMatchedError("category", category)); };

                return OperationResult.SuccessfulOperation();
            }

            return OperationResult.FaultedOperation(DeleteFromRangeOperation.InnerError);
        }
        public async Task<OperationResult> RemoveFromCategories(string[] Categories)
        {
            var transactionsFromCategories = await _StateStorage.GetEntitiesAsync(t => Categories.Contains(t.Category));

            if (!transactionsFromCategories.Success)
            {
                return OperationResult.FaultedOperation(transactionsFromCategories.InnerError);
            }

            return await _StateStorage.DeleteRangeAsync([.. transactionsFromCategories.Result!]);
        }
        public async Task<OperationResult> RestartDataAsync()
        {
            return await _StateStorage.ClearStorageAsync();
        }
    }
}
