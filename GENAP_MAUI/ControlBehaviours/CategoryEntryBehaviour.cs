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
            try
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
            catch (Exception)
            {
                // catching NullReferenceException in iOS without having a mac is like:

                if(e is null)
                {
                    Shell.Current.DisplayAlertAsync("DEBUG", "E is null", "a");
                    return;
                }
                   
                if(e.NewTextValue is null)
                {
                    Shell.Current.DisplayAlertAsync("DEBUG", "new text is null", "a");
                    return;
                }

                if(e.OldTextValue is null)
                {
                    Shell.Current.DisplayAlertAsync("DEBUG", "old text is null", "a");
                    return;
                }

                if(sender is null)
                {
                    Shell.Current.DisplayAlertAsync("DEBUG", "sender is null", "a");
                    return;
                }
            }
        }

    }
}
