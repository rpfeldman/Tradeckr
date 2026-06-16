namespace GENAP_MAUI
{
    public partial class MainPage : ContentPage
    {
        private bool _alreadyNavigated = false;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (_alreadyNavigated)
                    return;

                _alreadyNavigated = true;

                await Task.Yield();
                await Shell.Current.GoToAsync($"//{Routes.Dashboard}");

                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            catch (Exception x)
            {
                await Shell.Current.DisplayAlertAsync("debug", $"{x.Message}, {x.Data}, {x.InnerException}", "Aceptar");
            }
           
        }
    }
}
