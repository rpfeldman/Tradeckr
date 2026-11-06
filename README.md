
<div align="center">

# Tradeckr
<img src="docs/images/logo.png" alt="Tradeckr logo" width="120" />

**Your money, your control**

<!-- TODO: revisar/ajustar badges -->
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

Fun fact: the name is a mix of two words. My partner's name is Genaro, "Gena" for short, and at the beginning this was just an app for him. **Gena + App = GENAP.**

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

in progress...