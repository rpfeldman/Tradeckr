## What

I compared two ways of renaming a category in all of its transactions (this happens when the user edits a category name):

- **`RenameCategory_RowByRow`**: loads all the transactions with that category, changes the name one by one and saves with `SaveChanges()`. This means one `UPDATE` per transaction.

- **`RenameCategory_SetBased`**: uses `ExecuteUpdateAsync` to send a single `UPDATE` query to the database, without loading anything into memory.
## Conclusion

The set-based version is **around 4.5x faster** and uses **around 86x less memory** (64 KB vs 5.5 MB).

This makes sense: the row-by-row version has to load 500 objects, EF keeps a copy of each one to track the changes, and then it sends 500 different updates. It also makes the garbage collector work more, which is why its times vary so much. The set-based version uses almost the same memory no matter how many rows it updates.

# Result

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9711/25H2/2025Update/HudsonValley2)
Intel Core i5-9400F CPU 2.90GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK 10.0.401
  [Host]     : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3
  Job-CNUJVU : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v3

InvocationCount=1  UnrollFactor=1  

```
| Method                  | Names_Param          | Mean      | Error     | StdDev    | Median    | Rank | Gen0      | Allocated  |
|------------------------ |--------------------- |----------:|----------:|----------:|----------:|-----:|----------:|-----------:|
| RenameCategory_SetBased | (Clothing, Ropa)     |  6.048 ms | 0.1218 ms | 0.3476 ms |  5.989 ms |    1 |         - |   63.98 KB |
| RenameCategory_RowByRow | (Clothing, Ropa)     | 27.689 ms | 2.1999 ms | 6.1688 ms | 25.682 ms |    2 | 1000.0000 | 5530.88 KB |
|                         |                      |           |           |           |           |      |           |            |
| RenameCategory_SetBased | (Food, Comida)       |  6.387 ms | 0.1446 ms | 0.4125 ms |  6.308 ms |    1 |         - |   63.91 KB |
| RenameCategory_RowByRow | (Food, Comida)       | 30.192 ms | 2.4752 ms | 7.2203 ms | 28.840 ms |    2 |         - | 5527.32 KB |
|                         |                      |           |           |           |           |      |           |            |
| RenameCategory_SetBased | (Subs(...)ones) [30] |  6.409 ms | 0.1451 ms | 0.4140 ms |  6.322 ms |    1 |         - |   63.23 KB |
| RenameCategory_RowByRow | (Subs(...)ones) [30] | 28.645 ms | 2.2757 ms | 6.3436 ms | 27.034 ms |    2 | 1000.0000 | 5519.16 KB |




