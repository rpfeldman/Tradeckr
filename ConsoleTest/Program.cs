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
        }
    }

 
}
