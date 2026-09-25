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
            CurrenciesRatesService crs = new("");
        }
    }
}
