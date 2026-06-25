using Repositories;
using DomainModel;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleTest 
{
    internal class Program // TEMP
    {
        static async Task Main(string[] args)
        {
            EF_SQLite_StateStorageRepo<TransactionDto> repo = new("Test.db");


            TransactionDto[] transactions =
            {
                new TransactionDto() {Value = 10, Category = "hola", Date = DateOnly.FromDateTime(DateTime.Today), Depletion = true, Fixed = false},
                new TransactionDto() {Value = 5, Category = "hola", Date = DateOnly.FromDateTime(DateTime.Today), Depletion = true, Fixed = false},
                new TransactionDto() {Value = 5, Category = "hola", Date = DateOnly.FromDateTime(DateTime.Today), Depletion = true, Fixed = false},
                new TransactionDto() {Value = 5, Category = "hola", Date = DateOnly.FromDateTime(DateTime.Today), Depletion = true, Fixed = false},
                null
            };

            var op = await repo.SaveRangeAsync(transactions);

            if (op.Success)
            {
                Console.WriteLine("Op exitosa");
            }else { Console.WriteLine($"op fallida: {op.ErrorMessage}"); }
        }
    }
        
}
