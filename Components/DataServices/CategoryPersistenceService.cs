using DomainModel;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;

namespace DataServices
{
    public sealed class CategoryPersistenceService(IStateStorage<CategoryDto> stateStorage) // I implemented the category management service as a single class because it's much simpler than the Transaction service and doesn't need a multi-class structure.
    {
        private IStateStorage<CategoryDto> _StateStorage = stateStorage;
        public async Task<OperationResult<List<CategoryDto>>> GetCategoriesAsync()
        {
            var anyCategoriesOperation = await _StateStorage.AnyAsync();
            if (!anyCategoriesOperation.Success)
            {
                return OperationResult<List<CategoryDto>>.FaultedOperation(anyCategoriesOperation.ErrorMessage);
            }
            if (!anyCategoriesOperation.Result)
            {
                return OperationResult<List<CategoryDto>>.FaultedOperation("There's no categories available. At least one is required.");
            }

            var getCategoriesOperation = await _StateStorage.GetAllAsync();
            return getCategoriesOperation;
        }
        public async Task<OperationResult> AddCategoriesAsync(CategoryDto[] categories)
        {
            foreach (var category in categories)
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    return OperationResult.FaultedOperation($"{nameof(category.Name)} must have a content");
                }
                if (string.IsNullOrWhiteSpace(category.HexColor))
                {
                    return OperationResult.FaultedOperation($"{nameof(category.HexColor)} must have a content");
                }
            }

            var saveRangeOperation = await _StateStorage.SaveRangeAsync(categories);

            if (saveRangeOperation.Success)
            {
                if (saveRangeOperation.Result != categories.Length)
                {
                    return OperationResult.FaultedOperation("Some categories couldn't be saved. A few were and others weren't. Please review and try again");
                }

                return OperationResult.SuccessfulOperation();
            }
            return OperationResult.FaultedOperation(saveRangeOperation.ErrorMessage);
        }
        public async Task<OperationResult> RemoveCategoriesAsync(CategoryDto[] categories)
        {
            return await _StateStorage.DeleteRangeAsync(categories);
        }
        public async Task<OperationResult> UpdateCategoriesAsync(CategoryDto[] categories)
        {
            foreach (var category in categories)
            {
                if (string.IsNullOrWhiteSpace(category.Name))
                {
                    return OperationResult.FaultedOperation($"{nameof(category.Name)} must have a content");
                }
                if (string.IsNullOrWhiteSpace(category.HexColor))
                {
                    return OperationResult.FaultedOperation($"{nameof(category.HexColor)} must have a content");
                }
            }

            var updateRangeOperation = await _StateStorage.UpdateRangeAsync(categories);

            if (updateRangeOperation.Success)
            {
                if (updateRangeOperation.Result != categories.Length)
                {
                    return OperationResult.FaultedOperation("Some categories couldn't be updated. A few were and others weren't. Please review and try again");
                }

                return OperationResult.SuccessfulOperation();
            }
            return OperationResult.FaultedOperation(updateRangeOperation.ErrorMessage);
        }
        public async Task<OperationResult<bool>> HasCategories()
        {
            return await _StateStorage.AnyAsync();
        }
    }
}
