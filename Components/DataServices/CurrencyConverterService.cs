using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataServices
{
    public sealed class CurrencyConverterService
    {
        // TFU stands for 'Tradeckr Fixed Unit'
        // The idea is to use a normalized value to store every transaction, no matter its original currency.

        public static decimal TfuToCurrency(decimal value, CurrencyDto currency)
        {
            return value * currency.ConversionRate;
        }
        public static decimal CurrencyToTfu(decimal value, CurrencyDto currency) 
        {
            return value / currency.ConversionRate;
        }
    }
}
