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
        public async Task<OperationResult<IEnumerable<CategoryDto>>> GetCategoriesAsync()
        {
            var anyCategoriesOperation = await _StateStorage.AnyAsync();
            if (!anyCategoriesOperation.Success)
            {
                return OperationResult<IEnumerable<CategoryDto>>.FaultedOperation(anyCategoriesOperation.InnerError);
            }
            if (!anyCategoriesOperation.Result)
            {
                return OperationResult<IEnumerable<CategoryDto>>.FaultedOperation(ServiceErrors.NoElementsAvailable("categories"));
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
                    return OperationResult.FaultedOperation(ServiceErrors.EmptyFieldError(nameof(category.Name)));
                }
                if (string.IsNullOrWhiteSpace(category.HexColor))
                {
                    return OperationResult.FaultedOperation(ServiceErrors.EmptyFieldError(nameof(category.HexColor)));
                }
            }

            var saveRangeOperation = await _StateStorage.SaveRangeAsync(categories);

            if (saveRangeOperation.Success)
            {
                if (saveRangeOperation.Result != categories.Length)
                {
                    return OperationResult.FaultedOperation(ServiceErrors.PartialRegistrationError("categories", "saved"));
                }

                return OperationResult.SuccessfulOperation();
            }
            return OperationResult.FaultedOperation(saveRangeOperation.InnerError);
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
                    return OperationResult.FaultedOperation(ServiceErrors.EmptyFieldError(nameof(category.Name)));
                }
                if (string.IsNullOrWhiteSpace(category.HexColor))
                {
                    return OperationResult.FaultedOperation(ServiceErrors.EmptyFieldError(nameof(category.HexColor)));
                }
            }

            var updateRangeOperation = await _StateStorage.UpdateRangeAsync(categories);

            if (updateRangeOperation.Success)
            {
                if (updateRangeOperation.Result != categories.Length)
                {
                    return OperationResult.FaultedOperation(ServiceErrors.PartialRegistrationError("categories", "updated"));
                }

                return OperationResult.SuccessfulOperation();
            }
            return OperationResult.FaultedOperation(updateRangeOperation.InnerError);
        }
        public async Task<OperationResult<bool>> HasCategories()
        {
            return await _StateStorage.AnyAsync();
        }
    }
}
