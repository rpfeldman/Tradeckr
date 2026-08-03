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
            if (e.NewTextValue[^1] == ',' || e.NewTextValue[^1] == '.') { return; }
            
            if(!decimal.TryParse(entry.Text, out decimal value))
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
