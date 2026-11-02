using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DomainModel;
using System.Collections.ObjectModel;

namespace Benchmarks
{ 
    internal class Program
    {
        static void Main(string[] args)
        {
           BenchmarkRunner.Run<DataServicesBenchmarks>();
        }
    }
}
