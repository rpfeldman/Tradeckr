
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

        public GlobalResources()
        {
            // This is temporary, GlobalCategories should get the categories from a JSON file
            GlobalCategories =
            [
                new CategoryDto("Indumentaria", "#466C87", 0),

                new CategoryDto("Comida", "#F5E727", 1),

                new CategoryDto("Social", "#43EB28", 2),

                new CategoryDto("Gaming", "#9028EB", 3),

                new CategoryDto("Suscripciones", "#28EBB7", 4),
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
