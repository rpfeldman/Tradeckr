using DomainModel;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DataServices
{
    public static class ServiceErrors
    {
        // Since these messages will be seen by the user, they must come from a translation CSV file (or the localization system)

        public static InnerErrorDto ValueOutOfRangeError(int minBound, int maxBound) => new() { ErrorMessage = $"value must be in the range of {minBound:N2} to {maxBound:N2}", ErrorCode = 4 };
        public static InnerErrorDto PartialRegistrationError(string pluralEntity, string pastVerb) => new() { ErrorMessage = $"Some {pluralEntity} couldn't be {pastVerb}. A few were and others weren't. Please review and try again", ErrorCode = 7 };
        public static InnerErrorDto NotFoundId(int id) => new() { ErrorMessage = $"Unable to find a transaction with the following id: {id}", ErrorCode = 8 };
        public static InnerErrorDto NotFoundCategoryNameError(string categoryName) => new() { ErrorMessage = $"No movements with the category name {categoryName} were found", ErrorCode = 9};
        public static InnerErrorDto NoFixedTransactionsMatchedError(int id, int duration) => new() {ErrorMessage = $"Delete failed: no transactions matched in fixed transaction collection (CollectionId: {id}, FromDuration: {duration}) — 0 rows affected",  ErrorCode = 10};
        public static InnerErrorDto NoCollectionMatchedError(string entity, string identifier) => new() { ErrorMessage = $"Delete failed: no transaction collection matched '{entity}' '{identifier}' (0 rows affected)", ErrorCode = 11 };
        public static InnerErrorDto EmptyFieldError(string field) => new() { ErrorMessage = $"{field} must have a content", ErrorCode = 5 };
        public static InnerErrorDto NoElementsAvailable(string pluralEntity) => new() { ErrorMessage = $"There's no {pluralEntity} available. At least one is required.", ErrorCode = 12 };
        public static InnerErrorDto DurationOutOfRangeError { get => new() { ErrorMessage = "Duration must be greater than or equal to 1", ErrorCode = 6 }; }
    }
}

