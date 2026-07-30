using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.InnerComponents
{
    public sealed class GraphableTransactionDto(decimal value, string category, DateOnly date)
    {
        public decimal SignedValue { get; } = value;
        public string Category { get; } = category;
        public DateOnly Date { get; } = date;
    }
}
