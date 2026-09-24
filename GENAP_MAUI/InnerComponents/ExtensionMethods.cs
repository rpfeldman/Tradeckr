using DomainModel;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.InnerComponents
{
    public static class ExtensionMethods
    {
        public static void WriteLog(this OperationResult operation, string operationDescription)
        {
            if (!operation.Success)
            {
               Log.Error($"Error trying to '{operationDescription}'\n" +
                   $" Message:{operation.InnerError!.ErrorMessage}\n" 
                   + operation.InnerError!.ErrorDescription is not null ? $" Description: {operation.InnerError!.ErrorDescription}" : string.Empty +
                   $" Code: {operation.InnerError!.ErrorCode}");

                return;
            }

            Log.Information($"Successful operation '{operationDescription}'");
        }
        public static void WriteLog<T>(this OperationResult<T> operation, string operationDescription)
        {
            if (!operation.Success)
            {
                Log.Error($"Error trying to '{operationDescription}'\n" +
                   $" Message:{operation.InnerError!.ErrorMessage}\n" 
                   + operation.InnerError!.ErrorDescription is not null ? $" Description: {operation.InnerError!.ErrorDescription}" : string.Empty +
                   $" Code: {operation.InnerError!.ErrorCode}");

                return;
            }

            Log.Information($"Successful operation '{operationDescription}'");
        }
    }
}
