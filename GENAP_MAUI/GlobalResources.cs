
using CommunityToolkit.Mvvm.ComponentModel;
using GENAP_MAUI.InnerComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GENAP_MAUI
{
    public sealed class GlobalResources
    {
        // This is currently hardcoded, in the future it should get the name of the user from a JSON file that saves it
        public const string UserName = "Genaro";

        // Months name are also hardcoded, in the future they will be fetched by a CSV file with the translations
        public static string[] Months { get => ["Desconocido", "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"]; }

        // TimePeriod is split in 3: the enum (type-safe id), the display name (what the user sees), and the logic (per consumption point)
        // To add one: extend the enum, map its display name, handle its logic where consumed.
        public enum TimePeriodsEnum { Historical, HistoricalToday, Month, ThirtyDays, ThreeMonths, Semester, Year, Today }; 
        public Dictionary<TimePeriodsEnum, string> TimePeriods { get; }
        public List<KeyValuePair<TimePeriodsEnum, string>> TimePeriodsList { get; }

        // Same as TimePeriods

        public enum ColorsEnum { SteelBlue, Yellow, Green, Purple, Aqua, Coral, Red, Emerald, Cyan, Indigo, Magenta } 

        public static Dictionary<ColorsEnum, ColorDto> Colors { get; } = new()
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

        public static List<ColorDto> ColorList { get; } = [.. Colors.Values];
        public GlobalResources()
        {
            TimePeriods = new()
            {
                {TimePeriodsEnum.Historical, "Historico"},
                {TimePeriodsEnum.HistoricalToday, "Historico hasta hoy"},
                {TimePeriodsEnum.Year, "Ultimo año"},
                {TimePeriodsEnum.Semester, "Ultimo semestre"},
                {TimePeriodsEnum.ThreeMonths, "Ultimos 3 meses"},
                {TimePeriodsEnum.Month, "Este mes"},
                {TimePeriodsEnum.ThirtyDays, "Ultimos 30 dias"},
                {TimePeriodsEnum.Today, "Hoy"},
            };

            TimePeriodsList = [.. TimePeriods];
        }
    }
}
