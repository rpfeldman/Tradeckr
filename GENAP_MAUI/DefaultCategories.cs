using DomainModel;
using GENAP_MAUI.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI
{
    public static class DefaultCategories
    {
        public static string[] CategoriesStrings { get => AppResources.DefaultCategories.Split('_'); }
        public static CategoryDto[] DefaultCategoriesList = 
        [
            new CategoryDto() {Name = CategoriesStrings[0], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Coral].HexColor},

            new CategoryDto() {Name = CategoriesStrings[1], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Green].HexColor},

            new CategoryDto() {Name = CategoriesStrings[2], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Aqua].HexColor},

            new CategoryDto() {Name = CategoriesStrings[3], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Indigo].HexColor},

            new CategoryDto() {Name = CategoriesStrings[4], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Magenta].HexColor},

            new CategoryDto() {Name = CategoriesStrings[5], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.SteelBlue].HexColor},

            new CategoryDto() {Name = CategoriesStrings[6], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Purple].HexColor},

            new CategoryDto() {Name = CategoriesStrings[7], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Yellow].HexColor},

            new CategoryDto() {Name = CategoriesStrings[8], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.SteelBlue].HexColor},

            new CategoryDto() {Name = CategoriesStrings[9], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Emerald].HexColor},

            new CategoryDto() {Name = CategoriesStrings[10], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Cyan].HexColor},

            new CategoryDto() {Name = CategoriesStrings[11], HexColor=GlobalResources.Colors[GlobalResources.ColorsEnum.Red].HexColor},
        ];

        public const string TradingCategoryName = "Trading";
    }
}
