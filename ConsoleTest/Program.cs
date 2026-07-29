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
            var today = DateOnly.FromDateTime(DateTime.Today);

            var op = await repo.ProjectAsync<OnlyCategoryDto>(t => new() { CategoryName = t.Depletion.ToString() });

            if (op.Success)
            {
                foreach (var item in op.Result!)
                {
                    Console.WriteLine(item.GetType() + " " + item.CategoryName);
                }
            }
        }
    }

    public sealed class OnlyCategoryDto
    {
        public string CategoryName { get; set; }
    }

 
}
