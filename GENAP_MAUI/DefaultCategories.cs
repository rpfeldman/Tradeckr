using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI
{
    public static class DefaultCategories
    {
        public static CategoryDto[] DefaultCategoriesList = 
        [
            new CategoryDto() {Name = "Comida", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Coral].HexColor},

            new CategoryDto() {Name = "Sueldo", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Green].HexColor},

            new CategoryDto() {Name = "Indumentaria", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Aqua].HexColor},

            new CategoryDto() {Name = "Social", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Indigo].HexColor},

            new CategoryDto() {Name = "Suscripciones", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Magenta].HexColor},

            new CategoryDto() {Name = "Servicios vitales", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.SteelBlue].HexColor},

            new CategoryDto() {Name = "Entretenimiento", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Purple].HexColor},

            new CategoryDto() {Name = "Transporte", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Yellow].HexColor},

            new CategoryDto() {Name = "Varios", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.SteelBlue].HexColor},

            new CategoryDto() {Name = "Salud", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Emerald].HexColor},

            new CategoryDto() {Name = "Internet", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Cyan].HexColor},

            new CategoryDto() {Name = "Emergencias", HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Red].HexColor},
        ];

        public const string TradingCategoryName = "Trading";
    }
}
