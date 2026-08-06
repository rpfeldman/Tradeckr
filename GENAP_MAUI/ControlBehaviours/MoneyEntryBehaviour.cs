
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GENAP_MAUI.ControlBehaviours
{
    public sealed class MoneyEntryBehaviour : Behavior<Entry>
    {
        private bool _IsFormatting = false;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.TextChanged += Entry_TextChanged;

            // fucking hate samsung keyboard that doesn't recognise ',' as a valid decimal separator
            if (DeviceInfo.Manufacturer.Equals("Samsung", StringComparison.OrdinalIgnoreCase) && CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0] == ',')
            {
                bindable.Keyboard = Keyboard.Telephone;
            }
        }
        protected override void OnDetachingFrom(Entry bindable)
        {
            base.OnDetachingFrom(bindable);

            bindable.TextChanged -= Entry_TextChanged;
        }

        private void Entry_TextChanged(object? sender, TextChangedEventArgs e)
        { 
            if (_IsFormatting) { return; }

            var entry = (Entry)sender!;
            var culture = CultureInfo.CurrentCulture;
            var decimalSeparator = culture.NumberFormat.CurrencyDecimalSeparator[0];
            
            if (string.IsNullOrWhiteSpace(e.NewTextValue) || e.NewTextValue.Length <= 3 || e.NewTextValue[^1] == decimalSeparator) { return; } // cases in which formatting is not necessary

            _IsFormatting = true;

            if (!decimal.TryParse(entry.Text, out decimal value))
            {
                entry.Text = string.Empty;
                _IsFormatting = false;

                return;
            }

            entry.Dispatcher.Dispatch(() =>
            {
                entry.Text = value % 1 != 0 ? value.ToString("N2", culture) : value.ToString("N0", culture);
                _IsFormatting = false;
            });
        }
    }
}
