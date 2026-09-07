
using DomainModel;
using GENAP_MAUI.InnerComponents;
using Microsoft.Maui.Storage;

namespace GENAP_MAUI
{
    public static class GlobalResources
    {
        public static ManualResetEventSlim AppLoadingResetEvent { get; } = new(false); 

        public static string UserName { get => Preferences.Get(PreferenceKeys.UserNameKey, "Unknown"); }
        public static bool IsNewUser { get => Preferences.Get(PreferenceKeys.NewUserKey, true); }

         public static CurrencyDto DefaultCommonCurrency { get => Currencies[Preferences.Get(PreferenceKeys.CommonCurrencyKey, 0)]; }
         public static CurrencyDto DefaultTradingCurrency { get => Currencies[Preferences.Get(PreferenceKeys.TradingCurrencyKey, 0)]; }

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

        public static CurrencyDto[] Currencies { get; set; } =
            [
                new CurrencyDto() { CurrencyDisplayName = "Dólar Estadounidense", IsoCode = "usd", ConversionRate = 1, Id = 1 },

                new CurrencyDto() { CurrencyDisplayName = "Peso Argentino", IsoCode = "ars", ConversionRate = 1, Id = 2 },

                new CurrencyDto() { CurrencyDisplayName = "Peso uruguayo", IsoCode = "uyu", ConversionRate = 1, Id = 3 },

                new CurrencyDto() { CurrencyDisplayName = "Peso mexicano", IsoCode = "mxn", ConversionRate = 1, Id = 4 },

                new CurrencyDto() { CurrencyDisplayName = "Peso chileno", IsoCode = "clp", ConversionRate = 1, Id = 5 },

                new CurrencyDto() { CurrencyDisplayName = "Euro", IsoCode = "eur", ConversionRate = 1, Id = 6 },

                new CurrencyDto() { CurrencyDisplayName = "Libra esterlina", IsoCode = "gbp", ConversionRate = 1, Id = 7 },

                new CurrencyDto() { CurrencyDisplayName = "Yen japonés", IsoCode = "jpy", ConversionRate = 1, Id = 8 },

                new CurrencyDto() { CurrencyDisplayName = "Franco suizo", IsoCode = "chf", ConversionRate = 1, Id = 9 },

                new CurrencyDto() { CurrencyDisplayName = "Real brasileño", IsoCode = "brl", ConversionRate = 1, Id = 10 }
            ];
    }
}
