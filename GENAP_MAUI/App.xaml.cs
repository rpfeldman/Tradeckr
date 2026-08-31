using DataServices;
using NetworkServices;
using Microsoft.Extensions.DependencyInjection;
using DomainModel;

namespace GENAP_MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                CategoryPersistenceService categoryPersistenService = IPlatformApplication.Current!.Services.GetRequiredService<CategoryPersistenceService>();

                var anyCategoryOperation = await categoryPersistenService.HasCategories();

                // TO - DO: Apply a log system
                if (!anyCategoryOperation.Success)
                {
                    System.Diagnostics.Debug.WriteLine(anyCategoryOperation.InnerError?.ErrorMessage);
                }

                if (!anyCategoryOperation.Result)
                {
                    var setDefaultCategoriesOperation = await categoryPersistenService.AddCategoriesAsync(DefaultCategories.DefaultCategoriesList);

                    if(!setDefaultCategoriesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(setDefaultCategoriesOperation.InnerError?.ErrorMessage);
                    }
                }

                Application.Current?.UserAppTheme = Preferences.Get(PreferenceKeys.UserThemeKey, Application.Current?.UserAppTheme == AppTheme.Dark) ? AppTheme.Dark : AppTheme.Light;

                if (!NetworkMethods.CheckInternetConnection()) { System.Diagnostics.Debug.WriteLine("User does not have internet connection. Advacing without updating the currencies rates"); return; }

                var lastDayEntered = Preferences.Get(PreferenceKeys.LastDayEnteredKey, DateTime.Today);

                if(lastDayEntered != DateTime.Today)
                {
                    System.Diagnostics.Debug.WriteLine("Is a NEW DAY");
                }

                Preferences.Set(PreferenceKeys.LastDayEnteredKey, DateTime.Today);
            }
            catch (Exception x)
            {
                System.Diagnostics.Debug.WriteLine($"Seed failed: {x.Message}");
            }
        }
    }
}