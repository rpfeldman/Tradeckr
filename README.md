
<div align="center">

# Tradeckr
<img src="docs/images/Logo.png" alt="Tradeckr logo" width="120" />

**Your money, your control**

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![Platforms](https://img.shields.io/badge/platforms-Android%20%7C%20iOS%20%7C%20Windows%20%7C%20MacOs-informational)
![License](https://img.shields.io/badge/license-GPL--3.0-blue)
![Status](https://img.shields.io/badge/status-beta-orange)

</div>

---

## Table of Contents

- [Overview](#overview)
- [FAQ](#faq)
- [Architecture](#architecture)
- [Key Decisions](#key-decisions)
- [Benchmarks](#benchmarks)
- [Getting Started](#getting-started)
- [License](#license)
- [Credits](#credits)

---

## Overview

Cross-platform personal finance manager, built for day **traders**, but made for **everyone**!
### Features

- 📝 **Track your everyday money:** log your daily income and expenses in seconds.

- 🔁 **Set it and forget it:** salary, rent, subscriptions... register recurring transactions once and Tradeckr handles the rest.

- 📈 **Wins and losses, in the same place:** keep a record of your trading profits and losses, right next to your everyday finances.

- 📊 **See your decisions:** charts that show where your money goes, and how your trading is really doing.

- 🌎 **One place, many currencies:** see your money automatically converted to other currencies at any time.

- 💱 **Always up to date:** exchange rates update daily, so you can see how your money's real value changes over time.

- 🔒 **Your data stays with you:** everything is stored locally on your device. No accounts, no servers, nothing uploaded.

> [!NOTE]
> Tradeckr **does not handle real money** and is not a wallet. It only tracks your finances: no bank connections, no payments, no transfers.

### Screenshots

---

## FAQ

<details>
<summary><b>Is it paid <i>and</i> open source?</b></summary>

Yes, and that apparent contradiction is completely on purpose.

Of course, this means anyone can clone the repo and use the app for free. But the way I see it, that's not a problem. It's actually a sign of success: if someone takes the time to clone the repo, compile it and use it, it means I achieved what I wanted, which is people wanting to use Tradeckr.

So go ahead: clone it, learn from it, test it, build your own version. I don't mind at all.

And if Tradeckr ends up being part of your daily routine, consider buying it to support the project. But again, you're free to use it without paying.

</details>

<details>
<summary><b>What is it built with?</b></summary>

Tradeckr is built entirely in **C#** with **.NET 10 MAUI**, and uses these libraries:

- **Entity Framework Core + SQLite:** local data storage
- **CommunityToolkit.Mvvm:** MVVM infrastructure
- **CommunityToolkit.Maui.MediaElement:** the splash animation
- **LiveChartsCore + SkiaSharp:** charts
- **Serilog:** logging

</details>

<details>
<summary><b>Can I fork it?</b></summary>

Of course!

Feel free to fork it and make your own version.

And if you do, reach out and show me! I'd be glad to see it.

</details>

<details>
<summary><b>Can I fork it <i>and</i> sell it?</b></summary>

Yes, you can! The license allows it, but with a few rules:

- **It stays open source.** Your version must be released under the same license ([GPL-3.0](LICENSE)), and anyone who gets your app must be able to get its source code too. Taking Tradeckr and selling it as closed source is not allowed.
- **New name, new look.** "Tradeckr", its logo and its visual identity are not part of the license. Your version needs its own name and logo.
- **Give credit.** Keep the original copyright notice and mention that your version is based on Tradeckr.
- **Don't pretend to be the original.** It has to be clear that your version is a modified one, not the official Tradeckr.

All the details are in [NOTICE.md](NOTICE.md).

In short: build on top of it, improve it, even sell it, as long as it stays open and honest about where it came from.

</details>

<details>
<summary><b>What does GENAP means?</b></summary>

"GENAP" was the project's original name, before it became Tradeckr. For convenience, it was kept as the name of the main UI namespace.

Fun fact: the name is a mix of two words. My co-founder’s name is Genaro, “Gena” for short, and at the beginning this was just an app for him. Gena + App = GENAP.

</details>

<details>
<summary><b>Was it built with AI?</b></summary>

Mostly no. Tradeckr was solo-developed by me, handwriting code the old-fashioned way. No AI agents writing the app for me.

The whole architecture is mine: how responsibilities are split, the domains, the repository, the main systems. All of it was thought through and coded by me.

Where I did use AI was the visual side. I think I'm a pretty good programmer, but not much of an artist, so I used AI as a helper to make the app look good: styles, layouts and the look of the charts.

**In short:** the frontend (XAML and the LiveCharts code) was AI-assisted. The architecture and the core C# code were made by me.

</details>

<details>
<summary><b>Where is my data stored? Can you see it?</b></summary>

Tradeckr uses SQLite, which means all your data is stored **locally, on your device**. No accounts, no servers.

So no, neither I nor any company has access to your financial information. **I can't see your income or your expenses.**

The only thing worth knowing: depending on your device and settings, Android may automatically back up app data to **your own Google account**. That backup belongs to you, not to me or Tradeckr

</details>

<details>
<summary><b>Want to help?</b></summary>

You're more than welcome! There are a few ways to do it:

- 💡 **Ideas:** if you'd like a new feature, or you spot a system that could be optimized, tell me about it.
- 🛠️ **Code:** clone the repo, build the feature yourself and send me a pull request.
- 🌍 **Translations:** if you speak a language Tradeckr doesn't support yet, I'd love your help adding it.

For any of these, reach out at [ramirofeldman0@gmail.com](mailto:ramirofeldman0@gmail.com).

</details>

---

## Architecture

Tradeckr uses the **MVVM** pattern, which means that the interactive behavior of each XAML page lives in a stateful object called **ViewModel**.

The **Model** contains the domain classes, the generic repository interface, its implementation with **EF Core + SQLite**, and the services (transactions, currencies and categories).

The **View** has one ViewModel per interactive page and also has the global resources, the reusable ContentViews and the user preferences.

My intention was to keep the View completely separate from the Model, so the Model is fully reusable: you could build Tradeckr as a console app, with WPF, with Avalonia or with any other framework.

### Layers

```
┌─────────────────────────────────────────────────────┐
│                  GENAP_MAUI  (UI)                   │
│                                                     │
│   Views (XAML) ──►  ViewModels                      │
│   ContentViews · Resources · Preferences            │
│   MauiProgram  (DI composition root)                │
└────────────┬───────────────────────────┬────────────┘
             │                           │
             ▼                           ▼
┌────────────────────────┐   ┌────────────────────────┐
│      DataServices      │   │    NetworkServices     │
│  registration          │   │  exchange rates        │ ──► Rates API
│  projection            │   │  connectivity check    │
│  management            │   │                        │
│  categories · currency │   │                        │
└───────────┬────────────┘   └───────────┬────────────┘
            │                            │
            ▼                            │
┌────────────────────────┐               │
│      Repositories      │               │
│  IStateStorage<T>      │               │
│  EF Core + SQLite      │               │
└───────────┬────────────┘               │
            │                            │
            ▼                            ▼
┌─────────────────────────────────────────────────────┐
│                     DomainModel                     │
│   DTOs · IEntity · OperationResult<T> / Option<T>   │
└─────────────────────────────────────────────────────┘
```

### Domain Model

All entities implement `IEntity`, an interface that allows the repository to be generic.

**CategoryDto:** Represents the category as a displayable object, so it contains its name and a hex color to be used across the UI and charts.

**CurrencyDto:** Represents the concept of currency with its ISO code (e.g. `USD`, `ARS`) and its conversion rate.

**TransactionDto & FixedTransactionDto:** represent the transactions logged by the user: value, date, category, and whether it's an expense or an income. The domain doesn't attach the value to any currency; in Tradeckr's view implementation, values are stored in **TFU** (Tradeckr Fixed Unit) (see [Key Decisions](#key-decisions)).
`FixedTransactionDto` inherits from it and represents recurring transactions, adding a `Duration` and a `FixedTransactionId` that groups every occurrence of the same series.

### Generic Repository

At the beginning of Tradeckr's development, the repository wasn't generic: it was made just for the `TransactionDto` type. But when I wanted to add another type, I realized I'd have to COPY AND PASTE the same repository for it... so I had a little crashout weighing whether to do the easy thing and copy-paste the same repository for every entity, or do a refactor that would cost me some time but give me a better architectural result... my perfectionism won.

Persistence is defined by a single generic contract: `IStateStorage<TEntity>`. Any type that implements `IEntity` can be saved, read, updated or deleted through it, without the rest of the app knowing how or where it's stored.

The current implementation, `EF_SQLite_StateStorageRepo<T>`, uses EF Core + SQLite, and the same class serves transactions, categories and currencies. The only place that knows about it is `MauiProgram`, which registers it in the DI container. Everything else just asks for an `IStateStorage<T>`.

A few decisions behind it:

- **Short-lived `DbContext`:** every operation creates and disposes its own context, so no state is carried between operations.
- **`AsNoTracking()` on every read:** entities are never edited while tracked, so change tracking would only add overhead.
- **Projections in the database:** `ProjectAsync` and `ProjectByPredicateAsync` receive a selector that EF translates to SQL, so only the needed columns are read.
- **Row-by-row and set-based updates, side by side:** `UpdateRangeAsync` has both versions. Each one is used where it fits (see [Benchmarks](#benchmarks)).
- **No exceptions leak out:** every method returns an `OperationResult` or an `Option` (see [Error Handling](#error-handling)).

### Services

| Service                                      | Responsibility                                                                                                                                    | Depends on                                                  | What Tradeckr uses it for                                                                                  |
| -------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------- |
| `DataRegistrationService`                    | Registers expenses and incomes, both single and fixed (recurring)                                                                                 | `IStateStorage<TransactionDto>`                             | Logging new transactions and trades                                                                        |
| `DataProjectionService`                      | Reads and projects transactions: by date, month, year, category or custom predicate                                                               | `IStateStorage<TransactionDto>`, `CurrencyConverterService` | Dashboard, transactions list and charts                                                                    |
| `DataManagementService`                      | Edits and deletes transactions: single, by category or whole fixed series, plus bulk updates and data reset                                       | `IStateStorage<TransactionDto>`                             | Editing/deleting transactions, renaming categories and recalculating values when the base currency changes |
| `CategoryPersistenceService`                 | Category CRUD                                                                                                                                     | `IStateStorage<CategoryDto>`                                | Categories page and default categories                                                                     |
| `CurrencyPersistenceService`                 | Currency CRUD                                                                                                                                     | `IStateStorage<CurrencyDto>`                                | Storing currencies and their latest rates                                                                  |
| `CurrencyConverterService`                   | Converts values between TFU and any currency                                                                                                      | Nothing (pure static methods)                               | Showing every value in the currency the user picks                                                         |
| `CurrenciesRatesService` *(NetworkServices)* | Fetches exchange rates from [exchange-api](https://github.com/fawazahmed0/exchange-api) and applies them to currencies (doesn't persist anything) | `HttpClient`, `NetworkMethods`                              | Daily rate refresh on startup, onboarding and settings                                                     |
| `NetworkMethods` *(NetworkServices)*         | Checks internet connectivity                                                                                                                      | Nothing                                                     | Warning the user when they're offline                                                                      |

### ViewModels

| ViewModel | Responsibility | Depends on | Navigation |
|---|---|---|---|
| `MainDashboardPageViewModel` | Shows the current month's transactions and balance | `DataProjectionService` | Direct |
| `RegistTransactionPageViewModel` | Logs everyday expenses and incomes, single or fixed | `DataRegistrationService`, `CategoryPersistenceService` | Direct |
| `RegistTradeTransactionPageViewModel` | Logs trading profits and losses | `DataRegistrationService` | Direct |
| `TransactionCategoriesPageViewModel` | Adds, renames and deletes categories | `CategoryPersistenceService`, `DataManagementService` | Direct |
| `GraphsPageViewModel` | Fills the charts by time period and currency | `DataProjectionService`, `CategoryPersistenceService` | Direct |
| `TransactionsCollectionPageViewModel` | Lists transactions by time period and currency | `DataProjectionService` | Direct |
| `OnboardingpageViewModel` | First setup: user name, currencies and first rates download | `CurrencyPersistenceService`, `CurrenciesRatesService`, `NetworkMethods` | Direct |
| `TransactionPageViewModel` | Shows, edits and deletes a single transaction or its fixed series | `DataProjectionService`, `DataManagementService`, `CategoryPersistenceService` | Push |
| `SettingsPageViewModel` | Appearance, language and currency settings, plus bug reports | `DataManagementService`, `CurrencyPersistenceService`, `CurrenciesRatesService` | Push |
| `ClearStorageConfirmationPageViewModel` | Deletes all data and restores the default categories | `DataManagementService`, `CategoryPersistenceService` | Push |

Every ViewModel inherits from `BaseViewModel`, which centralizes navigation, and also depends on `CustomDialogsService` for alerts and dialogs.

**Direct vs. Push:**

- **Direct:** navigates straight to the page, replacing everything that was open before.
- **Push:** opens the page on top of the previous one, so the user can go back to where they were.

**Why did I choose one over the other?** Only for UX purposes: push pages don't have the bottom navigation bar, so they create the feeling of being in a "service" section of the app, like editing a transaction, clearing the storage or changing the settings.

### Error Handling

Early on, every layer threw exceptions and every file was full of `try/catch` blocks. And that worked (more or less), but then I realized I was throwing exceptions (which is a HEAVY process) for logic problems, like a null username. So I started to distinguish between logic problems and REAL exceptions... if SQLite doesn't work, THAT'S A PROBLEM. But if you manage to put a negative number into a numeric property of a domain object, that's not a technical error, that's a logic problem.

So I decided that **errors are values, not exceptions**. Every operation that can fail returns a result that says whether it worked, and why not if it didn't.

**The result types**:

- **`OperationResult` / `OperationResult<T>`:** a `Success` flag, the `Result` (if there is one) and an `InnerError`. Constructors are private, so the only way to build one is through `SuccessfulOperation()` or `FaultedOperation()`: you can't end up with a "successful operation with an error" by accident.
- **`Option<T>`:** for lookups where finding nothing is a valid outcome, not an error. `Some` or `None`, with a `Match` method to handle both cases.

All of them are `readonly struct`s, so they're immutable and cheap.

**`InnerErrorDto`, the shared error language:** it lives in the domain because every layer has to speak it. It carries:

- **`ErrorMessage`:** what the user sees.
- **`ErrorDescription`:** the technical details (exception message, stack trace...), meant for the logs.
- **`ErrorCode`:** a unique code across the whole app.

**Each layer has its own error catalog:**

| Catalog | Layer | Codes |
|---|---|---|
| `RepositorieErrors` | Repositories | 1–3 |
| `ServiceErrors` | DataServices | 4–12, 18 |
| `NetworkErrors` | NetworkServices | 13–17 |

**How it flows:** exceptions are only caught at the edges, in the repository and in the network calls, where they're translated into an `InnerErrorDto`. Services add their own validations and pass errors up untouched. And the ViewModels just check `Success`: if it failed, they show the `ErrorMessage` to the user and write the details to the log.

| Code | Error | Layer | Meaning |
|---|---|---|---|
| 1 | `DBProviderError` | Repositories | Couldn't connect to the storage system |
| 2 | `DBUpdateError` | Repositories | Couldn't save the changes |
| 3 | `TimeoutError` | Repositories | The storage operation took too long |
| 4 | `ValueOutOfRangeError` | DataServices | A value is outside the allowed range |
| 5 | `EmptyFieldError` | DataServices | A required field is empty |
| 6 | `DurationOutOfRangeError` | DataServices | A fixed transaction's duration is lower than 1 |
| 7 | `PartialRegistrationError` | DataServices | Only some items of a batch were saved or updated |
| 8 | `NotFoundId` | DataServices | No transaction exists with the given ID |
| 10 | `NoFixedTransactionsMatchedError` | DataServices | No transactions of the fixed series matched the deletion |
| 11 | `NoCollectionMatchedError` | DataServices | No transactions matched the category or series to delete |
| 12 | `NoElementsAvailable` | DataServices | There are no items available, and at least one is required |
| 13 | `InternetConnectionError` | NetworkServices | No internet connection |
| 14 | `HttpStatusCodeError` | NetworkServices | The request returned an unsuccessful status code |
| 15 | `HttpRequestError` | NetworkServices | The server couldn't be reached |
| 16 | `TimeoutError` | NetworkServices | The server took too long to respond |
| 17 | `JsonError` | NetworkServices | The server's response couldn't be read |
| 18 | `GenericArgumentError` | DataServices | An argument is not valid |

I know there are some missing codes, like `0` and `9`... I'm only human after all.

---

## Key Decisions

This section shows some of the decisions I had to make, in a summarized form.
### TFU (Tradeckr Fixed Unit)

When I decided to make Tradeckr multi-currency, I wanted a simple system where the user could see all their data converted to other currencies... but it wasn't that easy, because I ran into two problems: an economic one and a technical one.

**The economic problem: rates change. A LOT.**

As an Argentinian, I know how a currency's conversion rate can change every single day, so I couldn't just hardcode something like "1 USD = 1500 ARS": the app would be outdated in days.

I had two options: either push an update every single day, reading the market and changing the hardcoded values by hand... or use an API that does that for me. Thankfully, there's a free and open source exchange API maintained by [fawazahmed0](https://github.com/fawazahmed0/exchange-api).

**The technical problem: how do you show the same data in any currency?**

How could a user see their data in other currencies, regardless of the currency they originally entered it in?

And that's how the idea of the TFU came to my mind.

In Tradeckr's implementation, every transaction is stored in the same unit, no matter the currency it was registered in. That unit is the **TFU**, and it's always worth the same as the root currency.

**So how can you switch between currencies if every value is stored in the root currency?**

Because every currency is stored with its conversion rate updated daily against the root currency. Showing a transaction in any currency is just one multiplication away.
### The root currency is the trading currency

The TFU is anchored to the currency the user trades in. A day-trader doesn't want to see their money adjusted by anyone's inflation, they want to see that it's still **there**, in the currency they operate with. Every other currency is calculated against it.

**A feature for some, a bug for others.**

This decision has a side effect. Let's say you're an Argentinian who trades in USD but lives your daily life in ARS. If you look at a transaction from 6 months ago in pesos, you'll probably see a different amount than the one you registered, because it's converted with **today's** rate.

Maybe that's exactly what you want: it's the same **value**, just a different **amount** (thanks, inflation). Or maybe you want a perfect log of the exact amount of every transaction... in that case, you can just disable the daily rate update in the settings.
### Rates are stored, not fetched every time

Rates come from [exchange-api](https://github.com/fawazahmed0/exchange-api) and are updated once a day.

The easy way would've been fetching them every time the app starts... but that would've made internet **mandatory** just to open a finance tracker, and I don't believe software should work like that. An app that lives on your device should work on your device, with or without a connection.

So rates are persisted in the database. You only need internet **the very first time** you open Tradeckr, to get the initial rates. After that, you can open it on a plane, in the middle of nowhere or wherever you want: it just works with the last rates it got, and updates them the next time you're online.

(I couldn't use MAUI `Preferences` for this: they don't support `decimal`, and precision is the whole point of the TFU.)
### Changing the root currency recalculates EVERYTHING

This was the last feature I implemented before the beta, and I left it for the end on purpose: the root currency is the unit **every single transaction** is stored in. Touching it means touching all of them.

Think about it: if my root currency is USD and I change it to ARS without doing anything else, my 77 USD expense suddenly becomes a 77 ARS expense... a 99.9% discount nobody asked for.

So when the root currency changes, every transaction has to be converted. My first thought was the obvious one: load them all, convert them one by one and save them back. But a currency conversion is just the same multiplication applied to every row, so I went with a single set-based `UPDATE` that does it directly in the database, without loading anything into memory.

And of course, I measured it (see [Benchmarks](#benchmarks)).
### Categories are just names

At first, categories lived in the UI layer, with a `ColorDto` inside. But the user never *writes* a category: they pick it from a list they can edit. So categories became their own entity in the domain... and that's when the obvious "correct" idea showed up: make `TransactionDto` point to `CategoryDto` with a foreign key.

I didn't. That would've complicated my life for no reason.

`TransactionDto` only stores the category's **name** as a string, and that's it. No foreign keys, no relationships. And I kept it that way because each piece only knows what it needs to know:

- **`TransactionDto`** only knows how to put a name on a transaction's category. It doesn't know which categories exist, and it doesn't care.
- **`CategoryDto`** only knows its name and its hex color. It doesn't care about transactions, or about what the color is *called*.
- **`ColorDto`** stays in the View. It doesn't know anything about money or categories: it only knows it's a color with a display name, because only the UI cares about that.

The charts connect them by name: if there's a category called "Subscriptions", transactions with that name are painted with its color. If there isn't, they use a fallback color. Everything stays decoupled, and nothing breaks if a category is deleted.
### Fixed transactions are N transactions

A fixed transaction of 12 months is stored as **12 rows**, one per month. Simple enough... until you ask: how do you know those 12 rows belong together?

That's where I gave every fixed transaction **two identities**:

- **`Id`:** its own individual ID, like any other transaction.
- **`FixedTransactionId`:** the ID of the **series** it belongs to, shared by all 12 rows.

And each one also stores its `Duration` as a countdown: the first month has 12, the next one 11, and so on, so every occurrence knows how many months are left from it.

With that, the user can work at any level they want:

- Edit or delete **one** occurrence, using its `Id`.
- Delete **from this month on**, using the `FixedTransactionId` and the countdown.
- Delete the **whole series**, using only the `FixedTransactionId`.

One catch: EF Core can't generate values for columns that aren't keys, so I generate the series ID myself.

### Deleting everything should be hard

Clearing the storage is irreversible, so I made it intentionally annoying: a separate page with a big red warning, and a final confirmation before anything is deleted. If you clear your data, it's because you **really** wanted to.

---

in progress...

