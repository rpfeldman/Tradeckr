using DomainModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataServices
{
    public sealed class DataRegistrationService(IStateStorage<TransactionDto> StateStorage)
    {
        private IStateStorage<TransactionDto> _StateStorage = StateStorage;

        /// <summary>
        /// Generates and ID for fixed transactions collections
        /// </summary>
        /// <returns></returns>
        private async Task<OperationResult<int>> IdSetterForFixedTransactionsAsync() // As EF core can't generate values for non-keys properties and multiple instances will have the same ID, I decided make this method
        {
            var fixedTransactions = await _StateStorage.GetEntitiesAsync(t => t is FixedTransactionDto);

            if (!fixedTransactions.Success)
            {
                return OperationResult<int>.FaultedOperation($"{fixedTransactions.ErrorMessage}");
            }

            FixedTransactionDto? Last = (FixedTransactionDto?)fixedTransactions.Result?.OrderBy(t => (t as FixedTransactionDto)?.FixedTransactionId).LastOrDefault();

            if (Last is null)
            {
                return OperationResult<int>.SuccessfulOperation(0);
            }

            return OperationResult<int>.SuccessfulOperation(Last.FixedTransactionId + 1);
        }

        public async Task<OperationResult> RegistExpenseAsync(decimal value, DateOnly date, string category)
        {
            if (value < 1m || value > 1000000000m)
            {
                return OperationResult.FaultedOperation($"{nameof(value)} must be in the range of 1 to 1,000,000,000");
            }
            if (string.IsNullOrWhiteSpace(category))
            {
                return OperationResult.FaultedOperation($"{nameof(category)} must have a content");
            }

            var NewTransaction = new TransactionDto() { Value = value, Date = date, Category = category, Depletion = true, Fixed = false };

            return await _StateStorage.SaveAsync(NewTransaction);
        }

        public async Task<OperationResult> RegistFixedExpenseAsync(decimal value, DateOnly date, string category, int duration)
        {
            var GetCollectionId = await IdSetterForFixedTransactionsAsync();

            if (!GetCollectionId.Success)
            {
                return OperationResult.FaultedOperation(GetCollectionId.ErrorMessage);
            }
            if (value < 1m || value > 1000000000m)
            {
                return OperationResult.FaultedOperation($"{nameof(value)} must be in the range of 1 to 1,000,000,000");
            }
            if (string.IsNullOrWhiteSpace(category))
            {
                return OperationResult.FaultedOperation($"{nameof(category)} must have a content");
            }
            if (duration < 1)
            {
                return OperationResult.FaultedOperation($"{nameof(duration)} must be greater than or equal to 1 ");
            }

            List<FixedTransactionDto> transactions = new();
            for (int i = 0; i < duration; i++)
            {
                var NewTransaction = new FixedTransactionDto() { Value = value, Date = date, Depletion = true, Category = category, Fixed = true, Duration = (duration - i), FixedTransactionId = GetCollectionId.Result };

                transactions.Add(NewTransaction);
            }

            return await _StateStorage.SaveRangeAsync([.. transactions]);
        }

        public async Task<OperationResult> RegistIncomeAsync(decimal value, DateOnly date, string category)
        {
            if (value < 1m || value > 1000000000m)
            {
                return OperationResult.FaultedOperation($"{nameof(value)} must be in the range of 1 to 1,000,000,000");
            }
            if (string.IsNullOrWhiteSpace(category))
            {
                return OperationResult.FaultedOperation($"{nameof(category)} must have a content");
            }

            var NewTransaction = new TransactionDto() { Value = value, Date = date, Category = category, Depletion = false, Fixed = false };

            return await _StateStorage.SaveAsync(NewTransaction);
        }
        public async Task<OperationResult> RegistFixedIncomeAsync(decimal value, DateOnly date, string category, int duration)
        {
            var GetCollectionId = await IdSetterForFixedTransactionsAsync();

            if (!GetCollectionId.Success)
            {
                return OperationResult.FaultedOperation(GetCollectionId.ErrorMessage);
            }
            if (value < 1m || value > 1000000000m)
            {
                return OperationResult.FaultedOperation($"{nameof(value)} must be in the range of 1 to 1,000,000,000");
            }
            if (string.IsNullOrWhiteSpace(category))
            {
                return OperationResult.FaultedOperation($"{nameof(category)} must have a content");
            }
            if (duration < 1)
            {
                return OperationResult.FaultedOperation($"{nameof(duration)} must be greater than or equal to 1 ");
            }

            List<FixedTransactionDto> transactions = new();
            for (int i = 0; i < duration; i++)
            {
                var NewTransaction = new FixedTransactionDto() { Value = value, Date = date, Depletion = false, Category = category, Fixed = true, Duration = (duration - i), FixedTransactionId = GetCollectionId.Result };

                transactions.Add(NewTransaction);
            }

            return await _StateStorage.SaveRangeAsync([.. transactions]);
        }
    }
}
