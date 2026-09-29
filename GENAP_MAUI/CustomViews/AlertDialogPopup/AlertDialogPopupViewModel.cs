using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GENAP_MAUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GENAP_MAUI.CustomViews.AlertDialogPopup
{
    public sealed partial class AlertDialogPopupViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IPopupService _popupService;

        [ObservableProperty]
        public partial string Title { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Content { get; set;} = string.Empty;

        [ObservableProperty]
        public partial string ButtonText { get; set; } = string.Empty;

        public AlertDialogPopupViewModel(IPopupService popupService)
        {
            _popupService = popupService;
        }

        [RelayCommand]
        public async Task Close() 
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Title = (string)query[nameof(this.Title)];
            Content = (string)query[nameof(this.Content)];
            ButtonText = (string)query[nameof(this.ButtonText)];
        }
    }
}
