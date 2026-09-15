using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ContentViews.Graphs
{
    internal static class ChartFormat
    {
        public static string CompactCurrency(double value)
        {
            var abs = Math.Abs(value);
            return abs switch
            {
                >= 1_000_000 => $"{value / 1_000_000:0.#}M$",
                >= 10_000 => $"{value / 1_000:0.#}K$",
                _ => $"{value:N2}$"   
            };
        }
    }
}
