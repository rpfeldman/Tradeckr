
using DomainModel;
using GENAP_MAUI.InnerComponents;
using GENAP_MAUI.Resources.Strings;
using Microsoft.Maui.Storage;

namespace GENAP_MAUI
{
    public static class GlobalResources
    {
        public static ManualResetEventSlim AppLoadingResetEvent { get; } = new(false);

        public static string UserName { get => Preferences.Get(PreferenceKeys.UserNameKey, "Unknown"); }
        public static bool IsNewUser { get => Preferences.Get(PreferenceKeys.NewUserKey, true); }
        public static DateTime LastRateUpdate { get => Preferences.Get(PreferenceKeys.LastRateUpdateKey, DateTime.Now); }
        public static string LastRateUpdateFormatted => LastRateUpdate.ToString("dd/MM/yyyy");

        public static CurrencyDto DefaultCommonCurrency { get => Currencies[Preferences.Get(PreferenceKeys.CommonCurrencyKey, 0)]; }
        public static CurrencyDto DefaultTradingCurrency { get => Currencies[Preferences.Get(PreferenceKeys.TradingCurrencyKey, 0)]; }

        public static string[] Months { get => AppResources.Months.Split('_'); }

        // TimePeriod is split in 3: the enum (type-safe id), the display name (what the user sees), and the logic (per consumption point)
        // To add one: extend the enum, map its display name, handle its logic where consumed.
        public enum TimePeriodsEnum { Historical, HistoricalToday, Month, ThirtyDays, ThreeMonths, Semester, Year, Today };

        public static string[] TimePeriodsStrings { get => AppResources.TimePeriods.Split('_'); }
        public readonly static Dictionary<TimePeriodsEnum, string> TimePeriods = new(8)
         {
                {TimePeriodsEnum.Today, TimePeriodsStrings[0]},

                {TimePeriodsEnum.ThirtyDays, TimePeriodsStrings[1]},

                {TimePeriodsEnum.Month, TimePeriodsStrings[2]},

                {TimePeriodsEnum.ThreeMonths, TimePeriodsStrings[3]},

                {TimePeriodsEnum.Semester, TimePeriodsStrings[4]},

                {TimePeriodsEnum.Year, TimePeriodsStrings[5]},

                {TimePeriodsEnum.HistoricalToday, TimePeriodsStrings[6]},

                {TimePeriodsEnum.Historical, TimePeriodsStrings[7]},
         };
        
        public static List<KeyValuePair<TimePeriodsEnum, string>> TimePeriodsList { get => [.. TimePeriods]; }

        // Same as TimePeriods

        public enum ColorsEnum { SteelBlue, Yellow, Green, Purple, Aqua, Coral, Red, Emerald, Cyan, Indigo, Magenta } 

        public static string[] ColorsStrings { get => AppResources.Colors.Split('_'); }
        public readonly static Dictionary<ColorsEnum, ColorDto> Colors = new(16)
        {
            { ColorsEnum.SteelBlue, new ColorDto("#466C87", ColorsStrings[0]) },

            { ColorsEnum.Yellow, new ColorDto("#F1C40F", ColorsStrings[1]) },

            { ColorsEnum.Green, new ColorDto("#2ECC71", ColorsStrings[2]) },

            { ColorsEnum.Purple, new ColorDto("#9B59B6", ColorsStrings[3]) },

            { ColorsEnum.Aqua, new ColorDto("#1ABC9C", ColorsStrings[4]) },

            { ColorsEnum.Coral, new ColorDto("#E67E22", ColorsStrings[5]) },

            { ColorsEnum.Red, new ColorDto("#E74C3C", ColorsStrings[6]) },

            { ColorsEnum.Emerald, new ColorDto("#16A085", ColorsStrings[7]) },

            { ColorsEnum.Cyan, new ColorDto("#00BCD4", ColorsStrings[8]) },

            { ColorsEnum.Indigo, new ColorDto("#5C6BC0", ColorsStrings[9]) },

            { ColorsEnum.Magenta, new ColorDto("#E84393", ColorsStrings[10]) },
        }; 
       
        public static List<ColorDto> ColorList { get => [.. Colors.Values]; }

        // Same, UserAppTheme - a display name

        public readonly static Dictionary<AppTheme, string> AppThemes = new(2)
        {
            { AppTheme.Dark, "Oscuro" },
            { AppTheme.Light, "Claro" }
        }; 

        public static List<KeyValuePair<AppTheme, string>> AppThemesList { get => [.. AppThemes]; } 

        // In-memory currencies

        public static CurrencyDto[] Currencies { get; set; } =
            [
                new CurrencyDto() { CurrencyDisplayName = "Dólar Estadounidense", IsoCode = "USD", ConversionRate = 1, Id = 1 },

                new CurrencyDto() { CurrencyDisplayName = "Peso Argentino", IsoCode = "ARS", ConversionRate = 1, Id = 2 },

                new CurrencyDto() { CurrencyDisplayName = "Peso uruguayo", IsoCode = "UYU", ConversionRate = 1, Id = 3 },

                new CurrencyDto() { CurrencyDisplayName = "Peso mexicano", IsoCode = "MXN", ConversionRate = 1, Id = 4 },

                new CurrencyDto() { CurrencyDisplayName = "Peso chileno", IsoCode = "CLP", ConversionRate = 1, Id = 5 },

                new CurrencyDto() { CurrencyDisplayName = "Euro", IsoCode = "EUR", ConversionRate = 1, Id = 6 },

                new CurrencyDto() { CurrencyDisplayName = "Libra esterlina", IsoCode = "GBP", ConversionRate = 1, Id = 7 },

                new CurrencyDto() { CurrencyDisplayName = "Yen japonés", IsoCode = "JPY", ConversionRate = 1, Id = 8 },

                new CurrencyDto() { CurrencyDisplayName = "Franco suizo", IsoCode = "CHF", ConversionRate = 1, Id = 9 },

                new CurrencyDto() { CurrencyDisplayName = "Real brasileño", IsoCode = "BRL", ConversionRate = 1, Id = 10 }
            ];
    }
}
