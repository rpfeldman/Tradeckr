using Repositories;
using DomainModel;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography.X509Certificates;
using System.Net.WebSockets;
using DataServices;
using SQLitePCL;

namespace ConsoleTest 
{
    internal class Program 
    {
        static async Task Main(string[] args)
        {
            Batteries_V2.Init();

            EF_SQLite_StateStorageRepo<TransactionDto> repo = new("Test.db");
            DataProjectionService dps = new(repo);
            var today = DateOnly.FromDateTime(DateTime.Today);

            var AlltransactionsOp = await dps.ProjectTransactions<GraphableTransactionDto>(t => new(t.Depletion ? (t.Value * -1) : t.Value, t.Category, t.Date));

            foreach (var item in AlltransactionsOp.Result!)
            {
                Console.WriteLine($"{item.SignedValue:N2}$ | {item.Category} | {item.Date}");
            }
        }

        public readonly struct GraphableTransactionDto
        {
            public readonly string Category {  get; }
            public readonly decimal SignedValue { get; }
            public readonly DateOnly Date { get; }

            public GraphableTransactionDto(decimal value, string category, DateOnly date)
            {
                Category = category;
                SignedValue = value;
                Date = date;
            }
        }
    }
}
