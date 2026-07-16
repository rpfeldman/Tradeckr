using GENAP_MAUI.Pages.ConfirmationPages;
using GENAP_MAUI.Pages.MainNavigationBarPages;
using GENAP_MAUI.Pages.OnboardingPages;
using GENAP_MAUI.Pages.TransactionRelatedPages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI
{
    public static class Routes
    {
        public const string Dashboard = nameof(MainDashboardPage);
        public const string DefaultTransaction = nameof(RegistTransactionPage);
        public const string TradingTransactions = nameof(RegistTradeTransactionPage);
        public const string Categories = nameof(TransactionCategoriesPage);
        public const string Graphs = nameof(GraphsPage);
        public const string TransactionMenu = nameof(TransactionPage);
        public const string TransactionsList = nameof(TransactionsCollectionPage);
        public const string ClearStorageConfirmation = nameof(ClearStorageConfirmationPage);
        public const string Onboarding = nameof(OnboardingPage);
    }
}
