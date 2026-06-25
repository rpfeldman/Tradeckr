using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Text;

namespace DomainModel
{
    public readonly struct OperationResult
    {
        public bool Success { get; }
        public string? ErrorMessage { get; }

        private OperationResult(bool success, string? errorMessage)
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
    public readonly struct OperationResult<T> 
    {
        public T? Result { get; }
        public bool Success { get; }
        public string? ErrorMessage { get; }
        private OperationResult(bool success, string? errorMessage, T? value)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Result = value;
        }

        public static OperationResult<T> SuccessfulOperation(T result)
        {
            return new OperationResult<T>(true, null, result);
        }
        public static OperationResult<T> FaultedOperation(string? errorMessage)
        {
            return new OperationResult<T>(false, errorMessage, default);
        }
    }

    public readonly struct Option<T>
    {
        public bool HasValue { get; }
        public T? Value { get; }

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
