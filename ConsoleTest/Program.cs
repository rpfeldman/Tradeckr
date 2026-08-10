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

            CurrencyDto peso = new() { ConversionRate = 1498.2540m, CurrencyDisplayName = "Peso argentino", CurrencyId = 0, IsoCode = "ARS" };
            CurrencyDto dolar = new() { ConversionRate = 1, CurrencyDisplayName = "Dolar estadounidense", CurrencyId = 1, IsoCode = "USD" };
            CurrencyDto euro = new() { ConversionRate = 0.8662m, CurrencyDisplayName = "Euro", CurrencyId = 2, IsoCode = "EUR" };
            CurrencyDto yen = new() { ConversionRate = 159.1480m, CurrencyDisplayName = "Yen japones", CurrencyId = 3, IsoCode = "JPY" };
            CurrencyDto peso_uruguayo = new() { ConversionRate = 40.2576m, CurrencyDisplayName = "Peso urugayo", CurrencyId = 4, IsoCode = "UYU" };
            CurrencyDto rand = new() { ConversionRate = 16.1912m, CurrencyDisplayName = "Rand sudafricano", CurrencyId = 5, IsoCode = "ZAR" };
            CurrencyDto real = new() { ConversionRate = 5.1076m, CurrencyDisplayName = "Real brasileño", CurrencyId = 6, IsoCode = "BRL" };

            CurrencyDto[] Currencies = new CurrencyDto[] { peso, dolar, euro, yen, peso_uruguayo, rand, real };


            foreach (var item in Currencies)
            {
                Console.WriteLine($"Aqui estan tus movimientos representados en {item.CurrencyDisplayName} - {item.IsoCode}$ \n------\n");

                var getoperation = await dps.ProjectTransactions<TransactionDto>(t => new() { Value = CurrencyConverterService.TfuToCurrency(t.Value, item), Category = t.Category, Date = t.Date, Depletion = t.Depletion, Fixed = t.Fixed, Id = t.Id });

                if (getoperation.Success)
                {
                    foreach (var transaction in getoperation.Result!)
                    {
                        Console.WriteLine("se "+(transaction.Depletion ? "gasto " : "gano ")+$"{transaction.Value:N8} {item.IsoCode}$ en {transaction.Category}");
                    }
                }

                Console.WriteLine("\n------\n");
            }

            /*
            while (true)
            {
                Console.WriteLine("Registra un movimiento flaco, dale");
                Console.Write("Escribi la categoria pibe dale: ");
                string categoria = Console.ReadLine() ?? "Uncategorized";
                Console.Write("Escribi el valor en pesos: ");
                _ = decimal.TryParse(Console.ReadLine(), out decimal value);
                Console.Write("Decime si es un gasto o un ingreso pibe dale (1/0): ");
                int.TryParse(Console.ReadLine(), out int Ndepletion);
                bool depletion = Ndepletion == 1;

                if (depletion)
                {
                    var op = await drs.RegistExpenseAsync(CurrencyConverterService.CurrencyToTfu(value, peso), today, categoria);

                    if (op.Success)
                    {
                        Console.WriteLine("\n---\nGasto registrado con exito pibardo!\n---\n");
                    }
                    else
                    {
                        Console.WriteLine("Algo fallo xd"+" "+op.InnerError?.ErrorMessage);
                        break;
                    }

                    continue;
                }

                var op2 = await drs.RegistIncomeAsync(CurrencyConverterService.CurrencyToTfu(value, peso), today, categoria);

                if (op2.Success)
                {
                    Console.WriteLine("\n---\nIngreso registrado con exito pibardo!\n---\n");
                }
                else
                {
                    Console.WriteLine("Algo fallo xd" + " " + op2.InnerError?.ErrorMessage);
                    break;
                }
            }
            */

        }
    }
}
