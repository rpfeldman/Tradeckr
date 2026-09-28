# Tradeckr 📊

A **multi-platform finance management application for traders** built with C# and .NET. Tradeckr allows traders to track transactions, manage portfolios, and analyze trading activity across desktop, web, and mobile platforms.

> ⚠️ **Status**: Work in progress — core architecture complete, UI/UX refinement ongoing.

---

## Architecture

Tradeckr is built on a **layered, domain-driven architecture**:

```
GENAP_MAUI (UI Layer)
    ↓
Components/
├── DataServices (Business Logic)
├── Repositories (Data Access Abstraction)
├── NetworkServices (External API Integration)
└── DomainModel (Core Entities & DTOs)
```

### Key Components

| Layer | Purpose | Technologies |
|-------|---------|---------------|
| **Presentation** | Cross-platform UI | .NET MAUI (desktop, iOS, Android) |
| **Business Logic** | Domain operations | DataServices with OperationResult pattern |
| **Data Access** | Abstracted persistence | Generic repository pattern with EF Core & SQLite |
| **Domain Model** | Core entities | DTOs, strongly-typed Option/Result types |

---

## Features

### Core Functionality
- 📌 **Transaction Management** — Record trades with categories, currencies, and metadata
- 💾 **Multi-Storage Support** — SQLite (local), with extensible repository pattern
- 🎯 **Portfolio Tracking** — Aggregate trades by category and currency
- 🔄 **Result-Based Error Handling** — Railway-oriented result pattern for safe operations

### Architecture Highlights
- **Generic Repository Pattern** — `IStateStorage<T>` for any entity type
- **Dependency Inversion** — Swappable implementations (SQLite, future SQL Server, Dapper)
- **Type-Safe Operations** — `Option<T>` and `OperationResult<T>` for explicit error handling
- **Platform-Agnostic** — Shared business logic across UI platforms

---

## Project Structure

```
Components/
├── DomainModel/              # Core types
│   ├── TransactionDto.cs     # Trade record entity
│   ├── CategoryDto.cs        # Transaction category
│   ├── CurrencyDto.cs        # Currency type
│   └── Result&Option.cs      # Error handling types
│
├── Repositories/             # Data access layer
│   └── EF_SQLite_StateStorageRepo.cs   # Generic CRUD repository
│
├── DataServices/             # Business logic
│   └── [Service implementations]
│
└── NetworkServices/          # External integrations
    └── [API clients]

GENAP_MAUI/                  # .NET MAUI application
├── Pages/                   # Page views
├── ViewModels/              # MVVM view models
├── CustomViews/             # Reusable UI components
├── Resources/               # Strings, colors, fonts
└── MauiProgram.cs          # DI setup & bootstrapping
```

---

## Technology Stack

- **Language**: C# 12+
- **Frameworks**: 
  - .NET 8+ (Core runtime)
  - ASP.NET MAUI (Cross-platform UI)
  - Entity Framework Core (ORM)
- **Database**: SQLite (default), extensible to SQL Server/others
- **Architecture**: Layered + Domain-Driven Design
- **Error Handling**: Railway-oriented programming with custom `Result<T>` and `Option<T>`

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download) or later
- Visual Studio 2022+ or Rider

### Building

```bash
# Clone the repository
git clone https://github.com/rpfeldman/Tradeckr.git
cd Tradeckr

# Restore dependencies
dotnet restore

# Build the MAUI application
cd GENAP_MAUI
dotnet build
```

### Running

```bash
# Run desktop app
cd GENAP_MAUI
dotnet run
```

---

## Key Design Patterns

### 1. **Generic Repository Pattern**
```csharp
public interface IStateStorage<T> where T : class, IEntity
{
    Task<OperationResult> SaveAsync(T entity);
    Task<OperationResult<IEnumerable<T>>> GetAllAsync();
    Task<OperationResult> DeleteAsync(int id);
    // ... more operations
}
```

### 2. **Result-Based Error Handling**
Replaces exceptions with explicit `OperationResult<T>`:
```csharp
var result = await repository.SaveAsync(transaction);
if (result.IsSuccess)
{
    // Handle success
}
else
{
    // result.Error contains error details
}
```

### 3. **Dependency Injection**
MauiProgram.cs configures all services:
```csharp
builder.Services.AddSingleton<IStateStorage<Transaction>>(
    _ => new EF_SQLite_StateStorageRepo<Transaction>(path)
);
```

---

## Development Notes

- **Decimal Precision**: SQLite stores decimals as TEXT; precision configuration is database-agnostic
- **Async Throughout**: All repository operations are async-first
- **No Tracking**: Read operations use `.AsNoTracking()` for performance
- **Future Extensibility**: Repository layer designed to swap EF Core for Dapper/raw SQL as needed

---

## Roadmap

- [ ] Complete UI/UX for dashboard
- [ ] Add reporting & analytics views
- [ ] Implement data export (CSV, PDF)
- [ ] Add broker API integrations (market data)
- [ ] Web backend API (ASP.NET Core)
- [ ] Cloud sync across devices

---

## License

Licensed under the [Apache License 2.0](LICENSE).

---

## Author

**rpfeldman** — Building practical finance tools with clean architecture in mind.

**Learn more**: [GitHub Profile](https://github.com/rpfeldman)
