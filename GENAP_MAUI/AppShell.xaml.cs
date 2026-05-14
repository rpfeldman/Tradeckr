using GENAP_MAUI.Pages.MainNavigationBarPages;
using GENAP_MAUI.Pages.TransactionRelatedPages;

namespace GENAP_MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(Routes.Dashboard, typeof(MainDashboardPage));
            Routing.RegisterRoute(Routes.DefaultTransaction, typeof(RegistTransactionPage));
            Routing.RegisterRoute(Routes.TradingTransactions, typeof(RegistTradeTransactionPage));
            Routing.RegisterRoute(Routes.Categories, typeof(TransactionCategoriesPage));
            Routing.RegisterRoute(Routes.Graphs, typeof(GraphsPage));
            Routing.RegisterRoute(Routes.TransactionMenu, typeof(TransactionPage));
            Routing.RegisterRoute(Routes.TransactionsList, typeof(TransactionsCollectionPage));
        }
    }
}
