using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel
{
    public sealed class CurrencyDto : IEntity
    {
        public int Id { get; set; }
        public string IsoCode { get; set; } = string.Empty;
        public string CurrencyDisplayName { get; set; } = string.Empty;
        public decimal ConversionRate { get; set; }
    }
}
