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

            var today = DateOnly.FromDateTime(DateTime.Today);
            string isocode = "usd";

            CurrenciesRatesService crs = new(isocode);


            CurrencyDto[] currencies =
            [
                 new CurrencyDto() { CurrencyDisplayName = "Dólar Estadounidense", IsoCode = "usd", ConversionRate = 1 },
                 new CurrencyDto() { CurrencyDisplayName = "Peso Argentino", IsoCode = "ars", ConversionRate = 1494.2437m },
                 new CurrencyDto() { CurrencyDisplayName = "Peso uruguayo", IsoCode = "uyu", ConversionRate = 40.2120m },
                 new CurrencyDto() { CurrencyDisplayName = "Peso mexicano", IsoCode = "mxn", ConversionRate = 19.9070m },
                 new CurrencyDto() { CurrencyDisplayName = "Peso chileno", IsoCode = "clp", ConversionRate = 917.4312m },
                 new CurrencyDto() { CurrencyDisplayName = "Euro", IsoCode = "eur", ConversionRate = 0.8555m },
                 new CurrencyDto() { CurrencyDisplayName = "Libra esterlina", IsoCode = "gbp", ConversionRate = 0.7340m },
                 new CurrencyDto() { CurrencyDisplayName = "Yen japonés", IsoCode = "jpy", ConversionRate = 158.9775m },
                 new CurrencyDto() { CurrencyDisplayName = "Franco suizo", IsoCode = "chf", ConversionRate = 0.8014m },
                 new CurrencyDto() { CurrencyDisplayName = "Real brasileño", IsoCode = "brl", ConversionRate = 5.1623m },
                 new CurrencyDto() { CurrencyDisplayName = "El osurero", IsoCode = "osu", ConversionRate = 120m }
            ];

            Console.WriteLine("Old values"+Environment.NewLine);
            foreach (var item in currencies)
            {
                Console.WriteLine($"1 {item.CurrencyDisplayName} equivale a {item.ConversionRate:N3} usd$");
            }

            var updatecurrenciesop = await crs.UpdateCurrenciesRate(currencies, today);

            if (updatecurrenciesop.Success)
            {
                Console.WriteLine($"Se actualizaron {updatecurrenciesop.Result} de {currencies.Length}");
                Console.WriteLine(Environment.NewLine+"Current values"+Environment.NewLine);

                foreach (var item in currencies)
                {
                    Console.WriteLine($"1 {item.CurrencyDisplayName} equivale a {item.ConversionRate:N3} usd$");
                }
            }
            else { Console.WriteLine(updatecurrenciesop.InnerError!.ErrorMessage); }
           
        }
    }
}
