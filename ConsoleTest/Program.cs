using DataServices;
using DomainModel;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualBasic;
using NetworkServices;
using Repositories;
using Serilog;
using SQLitePCL;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleTest 
{
    internal class Program 
    {
        static async Task Main(string[] args)
        {
            Batteries_V2.Init();

            EF_SQLite_StateStorageRepo<CurrencyDto> repo = new("Test.db");
            CurrenciesRatesService crs = new("usd");

            var getRatesOperation = await crs.GetRatesAsync(DateOnly.FromDateTime(new DateTime(2026, 11, 10)));

            
            if(!getRatesOperation.Success)
            {
                Console.WriteLine($"Ha ocurrido un error: {getRatesOperation.InnerError!.ErrorMessage} - {getRatesOperation.InnerError.ErrorDescription}");
                return;
            }

            foreach (var item in getRatesOperation.Result!)
            {
                Console.WriteLine($"1,00$ 'usd' equivale a {item.Value:N2}$ '{item.Key}'");
            }

        }
    }
}
