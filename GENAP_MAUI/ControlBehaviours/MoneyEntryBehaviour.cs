
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GENAP_MAUI.ControlBehaviours
{
    public sealed class MoneyEntryBehaviour : Behavior<Entry>
    {
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
            var entry = (Entry)sender!;
            var culture = CultureInfo.CurrentCulture;
            var decimalSeparator = culture.NumberFormat.CurrencyDecimalSeparator[0];

            if (string.IsNullOrWhiteSpace(e.NewTextValue)) { return; }
            if(e.NewTextValue.Length > 15) { entry.Text = string.Empty; return; }
            if (e.NewTextValue[^1] == decimalSeparator) { return; }

            if (!decimal.TryParse(entry.Text, out decimal value))
            {
                entry.Text = string.Empty;
            }

            if(value % 1 != 0)
            {
                entry.Dispatcher.Dispatch(() =>
                {
                    entry.Text = value.ToString("N2", culture);
                });

                return;
            }

            entry.Dispatcher.Dispatch(() =>
            {
                entry.Text = value.ToString("N0", culture);
            });
        }
    }
}
