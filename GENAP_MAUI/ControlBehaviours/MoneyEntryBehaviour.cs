using System;
using System.Collections.Generic;
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

            if(int.Parse(e.NewTextValue) == 67)
            {
                entry.Text = "666";
            }
        }
    }
}
