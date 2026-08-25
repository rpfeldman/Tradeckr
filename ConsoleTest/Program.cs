using Repositories;
using DomainModel;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography.X509Certificates;
using System.Net.WebSockets;
using DataServices;
using SQLitePCL;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Globalization;
using System.Security;
using System.Runtime.CompilerServices;
using NetworkServices;

namespace ConsoleTest 
{
    internal class Program 
    {
        static async Task Main(string[] args)
        {
            Batteries_V2.Init();

            EF_SQLite_StateStorageRepo<TransactionDto> repo = new("Test.db");
            DataProjectionService dps = new(repo);
            DataManagementService dms = new(repo);
            DataRegistrationService drs = new(repo);

            string isocode = "sxo";

            CurrenciesRatesService crs = new(isocode);
            
            var today = DateOnly.FromDateTime(DateTime.Today);

            var getratesop = await crs.GetRatesAsync(today);

            if (getratesop.Success)
            {
                foreach (var item in getratesop.Result!)
                {
                    Console.WriteLine($"{isocode} vs {item.Key}: 1 : {item.Value:N3}");
                }
            }
            else { Console.WriteLine(getratesop.InnerError!.ErrorMessage); }
        }
    }
}
