using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public static class RepositorieErrors
    {
        public static InnerErrorDto DBProviderError(Exception x) {  return new() { ErrorCode = 1, ErrorMessage = "An error occurred while trying to connect to the storage system. Please try again", ErrorDescription = $"{x.Message} - {x.InnerException} - {x.Source} - {x.StackTrace}" }; }
        public static InnerErrorDto DBUpdateError(Exception x) { return new() { ErrorCode = 2, ErrorMessage = "An error occurred while trying to save the changes. Please try again", ErrorDescription = $"{x.Message} - {x.InnerException} - {x.Source} - {x.StackTrace}" }; }
        public static InnerErrorDto TimeoutError(Exception x) {  return new() { ErrorCode = 3, ErrorMessage = "The operation took too long. Please try again", ErrorDescription = $"{x.Message} - {x.InnerException} - {x.Source} - {x.StackTrace}" }; }
    }
}
