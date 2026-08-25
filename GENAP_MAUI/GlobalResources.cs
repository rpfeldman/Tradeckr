
using DomainModel;
using GENAP_MAUI.InnerComponents;
using Microsoft.Maui.Storage;

namespace GENAP_MAUI
{
    public static class GlobalResources
    {
        public static string UserName { get => Preferences.Get(PreferenceKeys.UserNameKey, "Unknown"); }
        public static bool IsNewUser { get => Preferences.Get(PreferenceKeys.NewUserKey, true); }

        public static KeyValuePair<CurrenciesEnum, CurrencyDto> DefaultCommonCurrency { get => CurrenciesList[Preferences.Get(PreferenceKeys.CommonCurrencyKey, 0)]; }
        public static KeyValuePair<CurrenciesEnum, CurrencyDto> DefaultTradingCurrency { get => CurrenciesList[Preferences.Get(PreferenceKeys.TradingCurrencyKey, 0)]; }

        // Months name are hardcoded, in the future they will be fetched by a CSV file with the translations
        public static string[] Months { get => ["Desconocido", "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"]; }

        // TimePeriod is split in 3: the enum (type-safe id), the display name (what the user sees), and the logic (per consumption point)
        // To add one: extend the enum, map its display name, handle its logic where consumed.
        public enum TimePeriodsEnum { Historical, HistoricalToday, Month, ThirtyDays, ThreeMonths, Semester, Year, Today }; 
        public readonly static Dictionary<TimePeriodsEnum, string> TimePeriods = new(8)
         {
                {TimePeriodsEnum.Today, "Hoy"},
                {TimePeriodsEnum.ThirtyDays, "Ultimos 30 dias"},
                {TimePeriodsEnum.Month, "Este mes"},
                {TimePeriodsEnum.ThreeMonths, "Ultimos 3 meses"},
                {TimePeriodsEnum.Semester, "Ultimo semestre"},
                {TimePeriodsEnum.Year, "Ultimo año"},
                {TimePeriodsEnum.HistoricalToday, "Historico hasta hoy"},
                {TimePeriodsEnum.Historical, "Proyeccion"},
         };
        
        public static List<KeyValuePair<TimePeriodsEnum, string>> TimePeriodsList { get => [.. TimePeriods]; }

        // Same as TimePeriods

        public enum ColorsEnum { SteelBlue, Yellow, Green, Purple, Aqua, Coral, Red, Emerald, Cyan, Indigo, Magenta } 

        public readonly static Dictionary<ColorsEnum, ColorDto> Colors = new(16)
        {
            { ColorsEnum.SteelBlue, new ColorDto("#466C87", "Azul plateado") },
            { ColorsEnum.Yellow, new ColorDto("#F1C40F", "Amarillo") },
            { ColorsEnum.Green, new ColorDto("#2ECC71", "Verde") },
            { ColorsEnum.Purple, new ColorDto("#9B59B6", "Morado") },
            { ColorsEnum.Aqua, new ColorDto("#1ABC9C", "Verde agua") },
            { ColorsEnum.Coral, new ColorDto("#E67E22", "Naranja") },
            { ColorsEnum.Red, new ColorDto("#E74C3C", "Rojo") },
            { ColorsEnum.Emerald, new ColorDto("#16A085", "Verde esmeralda") },
            { ColorsEnum.Cyan, new ColorDto("#00BCD4", "Celeste") },
            { ColorsEnum.Indigo, new ColorDto("#5C6BC0", "Lavanda") },
            { ColorsEnum.Magenta, new ColorDto("#E84393", "Magenta") },
        }; 
       
        public static List<ColorDto> ColorList { get => [.. Colors.Values]; } 

        public enum CurrenciesEnum { USD, ARS, EUR, GBP, JPY, CHF, BRL, CLP, UYU, MXN }

        // Display names are temporal
        // ConversionRates are also hardcoded from google finance values at 21/08/2026
        public readonly static Dictionary<CurrenciesEnum, CurrencyDto> Currencies = new(10)
        {
            { CurrenciesEnum.USD, new CurrencyDto() { CurrencyDisplayName = "Dólar Estadounidense", IsoCode = "usd", ConversionRate = 1 } },

            { CurrenciesEnum.ARS, new CurrencyDto() { CurrencyDisplayName = "Peso Argentino", IsoCode = "ars", ConversionRate = 1494.2437m } },

            { CurrenciesEnum.UYU, new CurrencyDto() { CurrencyDisplayName = "Peso uruguayo", IsoCode = "uyu", ConversionRate = 40.2120m } },

            { CurrenciesEnum.MXN, new CurrencyDto() { CurrencyDisplayName = "Peso mexicano", IsoCode = "mxn", ConversionRate = 19.9070m } },

            { CurrenciesEnum.CLP, new CurrencyDto() { CurrencyDisplayName = "Peso chileno", IsoCode = "clp", ConversionRate = 917.4312m } },

            { CurrenciesEnum.EUR, new CurrencyDto() { CurrencyDisplayName = "Euro", IsoCode = "eur", ConversionRate = 0.8555m } },

            { CurrenciesEnum.GBP, new CurrencyDto() { CurrencyDisplayName = "Libra esterlina", IsoCode = "gbp", ConversionRate = 0.7340m } },

            { CurrenciesEnum.JPY, new CurrencyDto() { CurrencyDisplayName = "Yen japonés", IsoCode = "jpy", ConversionRate = 158.9775m } },

            { CurrenciesEnum.CHF, new CurrencyDto() { CurrencyDisplayName = "Franco suizo", IsoCode = "chf", ConversionRate = 0.8014m } },

            { CurrenciesEnum.BRL, new CurrencyDto() { CurrencyDisplayName = "Real brasileño", IsoCode = "brl", ConversionRate = 5.1623m } },
        };
        

        public static List<KeyValuePair<CurrenciesEnum, CurrencyDto>> CurrenciesList { get => [.. Currencies]; }
    }
}
