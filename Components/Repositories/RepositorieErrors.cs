using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public static class RepositorieErrors
    {
        public static InnerErrorDto DBProviderError { get => new() { ErrorCode = 1, ErrorMessage = "An error occurred while trying to connect to the storage system. Please try again" }; }
        public static InnerErrorDto DBUpdateError { get => new() { ErrorCode = 2, ErrorMessage = "An error occurred while trying to save the changes. Please try again" }; }
        public static InnerErrorDto TimeoutError { get => new() { ErrorCode = 3, ErrorMessage = "The operation took too long. Please try again" }; }
    }
}
