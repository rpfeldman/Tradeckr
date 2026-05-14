
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace GENAP_MAUI
{
    public sealed class GlobalResources
    {
        public ObservableCollection<string> GlobalCategories { get; set; } = new();

        public GlobalResources()
        {
            // This is temporary, GlobalCategories should get the categories from a JSON file
            GlobalCategories =
            [
                "Indumentaria",
                "Comida",
                "Social",
                "Gaming",
                "Suscripciones",
            ];
        }
    }
}
