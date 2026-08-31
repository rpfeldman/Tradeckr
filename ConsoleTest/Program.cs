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

            EF_SQLite_StateStorageRepo<CurrencyDto> repo = new("Test.db");
            CurrencyPersistenceService cps = new(repo);


            CurrencyDto[] currencies = 
                [
                    new CurrencyDto() { CurrencyDisplayName = "Dólar Estadounidense", IsoCode = "usd", ConversionRate = 1, Id = 1 },
                    new CurrencyDto() { CurrencyDisplayName = "Peso Argentino", IsoCode = "ars", ConversionRate = 1324, Id = 2 },
                    new CurrencyDto() { CurrencyDisplayName = "Peso uruguayo", IsoCode = "uyu", ConversionRate = 40.2120m, Id = 3 },
                    new CurrencyDto() { CurrencyDisplayName = "Peso mexicano", IsoCode = "mxn", ConversionRate = 19.9070m, Id = 4 },
                    new CurrencyDto() { CurrencyDisplayName = "Peso chileno", IsoCode = "clp", ConversionRate = 917.4312m, Id = 5 },
                    new CurrencyDto() { CurrencyDisplayName = "Euro", IsoCode = "eur", ConversionRate = 0.8555m, Id = 6 },
                    new CurrencyDto() { CurrencyDisplayName = "Libra esterlina", IsoCode = "gbp", ConversionRate = 0.7340m, Id = 7 },
                    new CurrencyDto() { CurrencyDisplayName = "Yen japonés", IsoCode = "jpy", ConversionRate = 158.9775m, Id = 8 },
                    new CurrencyDto() { CurrencyDisplayName = "Franco suizo", IsoCode = "chf", ConversionRate = 0.8014m, Id = 9 },
                    new CurrencyDto() { CurrencyDisplayName = "Real brasileño", IsoCode = "brl", ConversionRate = 5.1623m, Id = 10 }
                ];


            var op = await cps.UpdateRange(currencies);

            if (op.Success)
            {
                Console.WriteLine("Operacion exitosa");
            }
            else { Console.WriteLine($"Hubo un error: {op.InnerError!.ErrorMessage}"); }
        }
    }
}
