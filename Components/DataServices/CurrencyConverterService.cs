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
            if(currency.ConversionRate <= 0)
                throw new ArgumentException($"{nameof(currency.ConversionRate)} must be a positive number");

            return value * currency.ConversionRate;
        }
        public static decimal CurrencyToTfu(decimal value, CurrencyDto currency) 
        {
            if(currency.ConversionRate <= 0)
                throw new ArgumentException($"{nameof(currency.ConversionRate)} must be a positive number");

            return value / currency.ConversionRate;
        }
        public static decimal CurrencyToCurrency(decimal value, CurrencyDto fromCurrency, CurrencyDto toCurrency) 
        {
            var convertedValue = CurrencyToTfu(value, fromCurrency);

            return convertedValue * toCurrency.ConversionRate;
        }
    }
}
