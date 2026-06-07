using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.InnerComponents
{
    public sealed class ColorDto
    {
        public string HexColor { get; set; }

        public string DisplayName { get; set; }

        public ColorDto(string color, string name)
        {
            HexColor = color;
            DisplayName = name;
        }
    }
}
