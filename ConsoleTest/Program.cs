using Repositories;
using DomainModel;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography.X509Certificates;
using System.Net.WebSockets;
using DataServices;

namespace ConsoleTest 
{
    internal class Program // TEMP
    {
        static async Task Main(string[] args)
        {
            EF_SQLite_StateStorageRepo<TransactionDto> repo = new("Test.db");
            var today = DateOnly.FromDateTime(DateTime.Today);
            DataRegistrationService drs = new(repo);
            DataManagementService dms = new(repo);
            DataProjectionService dps = new(repo);

            var op = await dps.GetAllByPredicateAsync(t => t is FixedTransactionDto);

            if (op.Success)
            {
                if(op.Result!.Count == 0) { Console.WriteLine("No se encontraron movimientos"); }
                foreach (var item in op.Result!)
                {
                    string dep = item.Depletion ? "Gasto:" : "Ingreso:";

                    Console.WriteLine($"{dep} {item.Value:N2}$ en {item.Category} el {item.Date}");
                }
            }else { Console.WriteLine($"Error: {op.ErrorMessage}"); }
        }
    }
}
