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
        public InnerErrorDto? InnerError { get; }

        private OperationResult(bool success, InnerErrorDto? innerError)
        {
            Success = success;
            InnerError = innerError;
        }

        public static OperationResult SuccessfulOperation()
        {
            return new OperationResult(true, null);
        }

        public static OperationResult FaultedOperation(InnerErrorDto? innerError)
        {
            return new OperationResult(false, innerError);
        }
    }
    public readonly struct OperationResult<T> 
    {
        public T? Result { get; }
        public bool Success { get; }
        public InnerErrorDto? InnerError { get; }
        private OperationResult(bool success, T? value, InnerErrorDto? innerError)
        {
            Success = success;
            Result = value;
            InnerError = innerError;
        }

        public static OperationResult<T> SuccessfulOperation(T result)
        {
            return new OperationResult<T>(true, result, null);
        }
        public static OperationResult<T> FaultedOperation(InnerErrorDto? innerError)
        {
            return new OperationResult<T>(false, default, innerError);
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

        public void Match(Action none, Action<T> some)
        {
            if (HasValue)
            {
                some.Invoke(Value!);
                return;
            }
            else
            {
                none.Invoke();
                return;
            }
        }
    }
}
