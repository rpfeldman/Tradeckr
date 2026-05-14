namespace GENAP_MAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Shell.Current.GoToAsync(Routes.Dashboard);
        }
    }
}
