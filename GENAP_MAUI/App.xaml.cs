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
                // TO - DO: Apply a log system

                if (GlobalResources.IsNewUser)
                {
                    CategoryPersistenceService categoryPersistenService = IPlatformApplication.Current!.Services.GetRequiredService<CategoryPersistenceService>();

                    var checkExistingCategoriesOperation = await categoryPersistenService.HasCategories();

                    if (!checkExistingCategoriesOperation.Success)
                    {
                        System.Diagnostics.Debug.WriteLine(checkExistingCategoriesOperation.InnerError?.ErrorMessage);
                        return;
                    }
                    if (!checkExistingCategoriesOperation.Result)
                    {
                        var setDefaultCategoriesOperation = await categoryPersistenService.AddCategoriesAsync(DefaultCategories.DefaultCategoriesList);

                        if (!setDefaultCategoriesOperation.Success)
                        {
                            System.Diagnostics.Debug.WriteLine(setDefaultCategoriesOperation.InnerError?.ErrorMessage);
                            return;
                        }
                    }

                    

                    return;
                }

                Application.Current?.UserAppTheme = Preferences.Get(PreferenceKeys.UserThemeKey, Application.Current?.UserAppTheme == AppTheme.Dark) ? AppTheme.Dark : AppTheme.Light;

                if (!NetworkMethods.CheckInternetConnection()) { System.Diagnostics.Debug.WriteLine("User does not have internet connection. Advacing without updating the currencies rates"); return; }

                var lastDayEntered = Preferences.Get(PreferenceKeys.LastDayEnteredKey, DateTime.Today);

                if(lastDayEntered != DateTime.Today)
                {
                    // TO - DO 
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