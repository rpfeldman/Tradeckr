
using GENAP_MAUI.Pages.ConfirmationPages;
using GENAP_MAUI.Pages.MainNavigationBarPages;
using GENAP_MAUI.Pages.TransactionRelatedPages;

namespace GENAP_MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(Routes.TransactionMenu, typeof(TransactionPage));
            Routing.RegisterRoute(Routes.ClearStorageConfirmation, typeof(ClearStorageConfirmationPage));


            if (GlobalResources.IsNewUser)
            {
                this.GoToAsync($"//{Routes.Onboarding}");
                return;
            }

            this.GoToAsync($"//{Routes.Dashboard}");
        }
    }
}
