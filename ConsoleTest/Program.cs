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

namespace ConsoleTest 
{
    internal class Program 
    {
        static async Task Main(string[] args)
        {
            Batteries_V2.Init();

            EF_SQLite_StateStorageRepo<TransactionDto> repo = new("Test.db");
            DataProjectionService dps = new(repo);
            DataRegistrationService drs = new(repo);
            var today = DateOnly.FromDateTime(DateTime.Today);
        }
    }
}
