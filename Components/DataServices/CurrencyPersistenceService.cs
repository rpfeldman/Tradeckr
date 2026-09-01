using DomainModel;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Json.Internal;
using Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataServices
{
    public sealed class CurrencyPersistenceService(IStateStorage<CurrencyDto> stateStorage)
    {
        private IStateStorage<CurrencyDto> _StateStorage = stateStorage;

        public async Task<OperationResult> AddRangeAsync(CurrencyDto[] currencies)
        {
            foreach (var currency in currencies)
            {
                if(string.IsNullOrWhiteSpace(currency.IsoCode))
                {
                    return OperationResult.FaultedOperation(ServiceErrors.EmptyFieldError(nameof(currency.IsoCode)));
                }

                if (string.IsNullOrWhiteSpace(currency.CurrencyDisplayName))
                {
                    return OperationResult.FaultedOperation(ServiceErrors.EmptyFieldError(nameof(currency.CurrencyDisplayName)));
                }

                if (currency.ConversionRate < 0)
                {
                    return OperationResult.FaultedOperation(ServiceErrors.GenericArgumentError(nameof(currency.ConversionRate)));
                }
            }

            var addRangeOperation = await _StateStorage.SaveRangeAsync(currencies);

            if (!addRangeOperation.Success)
            {
                return OperationResult.FaultedOperation(addRangeOperation.InnerError);
            }

            return OperationResult.SuccessfulOperation();
        }

        public async Task<OperationResult<int>> UpdateRangeAsync(CurrencyDto[] currencies)
        {
            foreach (var currency in currencies)
            {
                if (string.IsNullOrWhiteSpace(currency.IsoCode))
                {
                    return OperationResult<int>.FaultedOperation(ServiceErrors.EmptyFieldError(nameof(currency.IsoCode)));
                }

                if (string.IsNullOrWhiteSpace(currency.CurrencyDisplayName))
                {
                    return OperationResult<int>.FaultedOperation(ServiceErrors.EmptyFieldError(nameof(currency.CurrencyDisplayName)));
                }

                if (currency.ConversionRate < 0)
                {
                    return OperationResult<int>.FaultedOperation(ServiceErrors.GenericArgumentError(nameof(currency.ConversionRate)));
                }
            }

            var updateRangeOperation = await _StateStorage.UpdateRangeAsync(currencies);

            if (updateRangeOperation.Success)
            {
                if (updateRangeOperation.Result != currencies.Length)
                {
                    return OperationResult<int>.FaultedOperation(ServiceErrors.PartialRegistrationError("categories", "updated"));
                }

                return updateRangeOperation;
            }

            return updateRangeOperation;
        }

        public async Task<OperationResult> RemoveAsync(int id)
        {
            return await _StateStorage.DeleteAsync(id);
        }

        public async Task<OperationResult> RemoveRangeAsync(CurrencyDto[] currencies)
        {
            return await _StateStorage.DeleteRangeAsync(currencies);
        }

        public async Task<OperationResult<IEnumerable<CurrencyDto>>> GetAllAsync()
        {
            return await _StateStorage.GetAllAsync();
        }

        public async Task<OperationResult<bool>> HasCurrencies()
        {
            return await _StateStorage.AnyAsync();
        }
    }
}
