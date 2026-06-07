
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
        public ObservableCollection<CategoryDto> GlobalCategories { get; set; } = new();


        // TimePeriod is split in 3: the enum (type-safe id), the display name (what the user sees), and the logic (per consumption point)
        // To add one: extend the enum, map its display name, handle its logic where consumed.
        public enum TimePeriodsEnum { Historical, HistoricalToday, Month, ThirtyDays, ThreeMonths, Semester, Year, Today }; 
        public Dictionary<TimePeriodsEnum, string> TimePeriods { get; }

        public List<KeyValuePair<TimePeriodsEnum, string>> TimePeriodsList { get; }

        // Same as TimePeriods

        public enum ColorsEnum { SteelBlue, Yellow, Green, Purple, Aqua, Coral, Red, Emerald, Cyan, Indigo, Magenta } 

        public Dictionary<ColorsEnum, ColorDto> Colors { get; }

        public List<KeyValuePair<ColorsEnum, ColorDto>> ColorList { get; }

        public GlobalResources()
        {
            Colors = new()
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

            ColorList = [.. Colors];

            // This is temporary, GlobalCategories should get the categories from a JSON file
            GlobalCategories =
            [
                new CategoryDto("Indumentaria", Colors[ColorsEnum.Aqua].HexColor, 0),

                new CategoryDto("Comida", Colors[ColorsEnum.Yellow].HexColor, 1),

                new CategoryDto("Social", Colors[ColorsEnum.Green].HexColor, 2),

                new CategoryDto("Gaming", Colors[ColorsEnum.Purple].HexColor, 3),

                new CategoryDto("Suscripciones", Colors[ColorsEnum.Coral].HexColor, 4),
            ];


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
