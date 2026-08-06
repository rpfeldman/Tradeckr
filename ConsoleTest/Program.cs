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

            Currency dolar = new() { ConversionRate = 1.61m, CurrencyCode = "USD", Id = 0 };
            Currency peso = new() { ConversionRate = 2447.2m, CurrencyCode = "ARS", Id = 1 };
            Currency euro = new() { ConversionRate = 1.395m, CurrencyCode = "EUR", Id = 2 };

            var MovimientosDolares = await dps.ProjectTransactions<TransactionDto>(t => new() {Category = t.Category, Date = t.Date, Depletion = t.Depletion, Fixed = t.Fixed, Id = t.Id, Value = CurrencyConverter.FtuToCurrency(dolar, t.Value) });
            var MovimientosEuros = await dps.ProjectTransactions<TransactionDto>(t => new() { Category = t.Category, Date = t.Date, Depletion = t.Depletion, Fixed = t.Fixed, Id = t.Id, Value = CurrencyConverter.FtuToCurrency(euro, t.Value) });
            var MovimientosPesos = await dps.ProjectTransactions<TransactionDto>(t => new() { Category = t.Category, Date = t.Date, Depletion = t.Depletion, Fixed = t.Fixed, Id = t.Id, Value = CurrencyConverter.FtuToCurrency(peso, t.Value) });

            Console.WriteLine("Movimientos en pesos: ");
            foreach (var item in MovimientosPesos.Result!)
            {
                Console.WriteLine("un " + (item.Depletion ? "gasto":"ingreso") + $" de {item.Value:N2} ARS$ en {item.Category}");
            }
            Console.WriteLine("\n---\nMovimientos en euros:");
            foreach (var item in MovimientosEuros.Result!)
            {
                Console.WriteLine("un " + (item.Depletion ? "gasto" : "ingreso") + $" de {item.Value:N2} EUR$ en {item.Category}");
            }
            Console.WriteLine("\n---\nMovimientos en dolares:");
            foreach (var item in MovimientosDolares.Result!)
            {
                Console.WriteLine("un " + (item.Depletion ? "gasto" : "ingreso") + $" de {item.Value:N2} USD$ en {item.Category}");
            }


            /*
            while (true)
            {
                Console.WriteLine("Registra un movimiento flaco, dale");
                Console.Write("Escribi la categoria pibe dale: ");
                string categoria = Console.ReadLine() ?? "Uncategorized";
                Console.Write("Escribi el valor en pesos: ");
                _ = decimal.TryParse(Console.ReadLine(), out decimal value);
                Console.Write("Decime si es un gasto o un ingreso pibe dale (0/1): ");
                int.TryParse(Console.ReadLine(), out int Ndepletion);
                bool depletion = Ndepletion == 1;

                if (depletion)
                {
                    var op = await drs.RegistExpenseAsync(CurrencyConverter.CurrencyToFtu(peso, value), today, categoria);

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

                var op2 = await drs.RegistIncomeAsync(CurrencyConverter.CurrencyToFtu(peso, value), today, categoria);

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

        public sealed class Currency
        {
            public decimal ConversionRate { get; set; }
            public int Id { get; set; }
            public string CurrencyCode { get; set; } = string.Empty;
        }

        public sealed class CurrencyConverter
        {
            public static decimal FtuToCurrency(Currency currency, decimal value)
            {
                return value * currency.ConversionRate;
            }

            public static decimal CurrencyToFtu(Currency currency, decimal value)
            {
                return value / currency.ConversionRate;
            }
        }
    }
}
