using System.Globalization;
using System.Text.RegularExpressions;

namespace GENAP_MAUI.ContentViews.Graphs
{
    internal static class ChartFormat
    {
        private static readonly Regex YearPart = new(@"[^dM]*y+[^dM]*", RegexOptions.Compiled);

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
        public static string ShortDayMonth(DateOnly date)
        {
            var culture = CultureInfo.CurrentCulture;
            var pattern = YearPart.Replace(culture.DateTimeFormat.ShortDatePattern, string.Empty);
            return date.ToString(pattern, culture);
        }
    }
}
