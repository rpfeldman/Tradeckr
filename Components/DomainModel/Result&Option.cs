using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Text;

namespace DomainModel
{
    public class OperationResult
    {
        public readonly bool Success;
        public readonly string? ErrorMessage;

        protected OperationResult(bool success, string? errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static OperationResult SuccessfulOperation()
        {
            return new OperationResult(true, null);
        }

        public static OperationResult FaultedOperation(string? errorMessage)
        {
            return new OperationResult(false, errorMessage);
        }
    }
    public sealed class OperationResult<T> : OperationResult
    {
        public readonly T? Result;
        private OperationResult(bool success, string? errorMessage, T? value) : base(success, errorMessage)
        {
            Result = value;
        }

        public static OperationResult<T> SuccessfulOperation(T result)
        {
            return new OperationResult<T>(true, null, result);
        }
        public static new OperationResult<T> FaultedOperation(string? errorMessage)
        {
            return new OperationResult<T>(false, errorMessage, default);
        }
    }

    public sealed class Option<T>
    {
        public readonly bool HasValue;
        public readonly T? Value;

        private Option(bool hasValue, T? value)
        {
            HasValue = hasValue;
            Value = value;
        }

        public static Option<T> Some(T? value)
        {
            return new Option<T>(true, value);
        }
        public static Option<T> None()
        {
            return new Option<T>(false, default);
        }
    }
}
