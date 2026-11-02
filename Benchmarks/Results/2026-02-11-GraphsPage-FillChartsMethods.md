## What

Refactored the data-loading logic of `GraphsPageViewModel`, which fetches the transactions used to populate the charts.

Previously, each refresh issued **four separate database queries** (daily expenses, trading losses, daily income and trading gains) for the same time period. The new implementation issues **a single query**, projected directly to `GraphableTransactionDto` at the database level, and splits the result in memory.

Three implementations were benchmarked with BenchmarkDotNet against a real SQLite file (~160 transactions of mixed types):

- **`OldMethod`**: original implementation, four queries per refresh (baseline).
- **`OptimizedMethodManual`**: single query, in-memory split using manual loops.
- **`OptimizedMethodLINQ`**: single query, in-memory split using LINQ.

## Conclusion

`OptimizedMethodLINQ` is the fastest and most memory-efficient implementation across every time period tested, outperforming even the manual-loop version.

In the most common use case (the current month), it loads the chart data **2.6× faster (−61.4% time) while allocating 71% less memory** than the original implementation.
# Result


```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9711/25H2/2025Update/HudsonValley2)
Intel Core i5-9400F CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3


```
| Method                | timePeriodParam | Mean       | Error    | StdDev   | Rank | Gen0     | Gen1    | Allocated |
|---------------------- |---------------- |-----------:|---------:|---------:|-----:|---------:|--------:|----------:|
| OptimizedMethodLINQ   | Historical      |   600.0 μs |  1.92 μs |  1.60 μs |    1 |  41.0156 |  7.8125 | 188.58 KB |
| OptimizedMethodManual | Historical      |   842.5 μs |  3.18 μs |  2.66 μs |    2 |  54.6875 | 11.7188 | 266.94 KB |
| OldMethod             | Historical      |   894.9 μs |  8.18 μs |  6.39 μs |    3 |  62.5000 | 11.7188 | 290.81 KB |
|                       |                 |            |          |          |      |          |         |           |
| OptimizedMethodLINQ   | Month           |   468.7 μs |  2.48 μs |  2.32 μs |    1 |  14.6484 |  1.9531 |  70.52 KB |
| OptimizedMethodManual | Month           |   530.0 μs |  4.43 μs |  3.70 μs |    2 |  15.6250 |  1.9531 |  73.31 KB |
| OldMethod             | Month           | 1,212.6 μs |  7.08 μs |  6.28 μs |    3 |  50.7813 |  7.8125 | 241.95 KB |
|                       |                 |            |          |          |      |          |         |           |
| OptimizedMethodLINQ   | Year            | 2,150.4 μs | 12.92 μs | 10.79 μs |    1 |  93.7500 | 31.2500 | 464.13 KB |
| OptimizedMethodManual | Year            | 2,395.2 μs |  8.42 μs |  7.03 μs |    2 | 109.3750 | 39.0625 | 562.64 KB |
| OldMethod             | Year            | 3,152.0 μs | 58.56 μs | 51.91 μs |    3 | 132.8125 | 39.0625 | 628.52 KB |


