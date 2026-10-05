using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GENAP_MAUI.ControlBehaviours
{
    public sealed class CategoryEntryBehaviour : Behavior<Entry>
    {
        private bool IsFormatting = false;

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
            if (IsFormatting){ return; }

            IsFormatting = true;

            var entry = (Entry)sender!;

            if(entry.Text.Length > BoundsConst.CategoryNameLimit)
            {
                entry.Text = entry.Text[..^1];
                IsFormatting = false;

                return;
            }

            IsFormatting = false;
        }

    }
}
