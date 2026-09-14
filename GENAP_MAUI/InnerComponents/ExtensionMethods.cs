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
               Log.Error($"Error trying to '{operationDescription}'\n {operation.InnerError!.ErrorMessage}\n Code: {operation.InnerError!.ErrorCode}");
                return;
            }

            Log.Information($"Successful operation '{operationDescription}'");
        }
        public static void WriteLog<T>(this OperationResult<T> operation, string operationDescription)
        {
             if (!operation.Success)
            {
               Log.Error($"Error trying to '{operationDescription}'\n {operation.InnerError!.ErrorMessage}\n Code: {operation.InnerError!.ErrorCode}");
                return;
            }

            Log.Information($"Successful operation '{operationDescription}'");
        }
    }
}
