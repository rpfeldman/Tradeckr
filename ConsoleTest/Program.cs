using Repositories;
using DomainModel;
using System.IO;
using Microsoft.VisualBasic;
using DataServices;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ConsoleTest 
{
    internal class Program // TEMP
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var path = "test.db";
            var repo = new EF_SQLite_StateStorage(path, [14, 2]);
            var DPS = new DataProjectionService(repo);
            var DRS = new DataRegistrationService(repo);
            var DMS = new DataManagementService(repo);
            var today = DateOnly.FromDateTime(DateTime.Today);


            
        }
    }
        
}
