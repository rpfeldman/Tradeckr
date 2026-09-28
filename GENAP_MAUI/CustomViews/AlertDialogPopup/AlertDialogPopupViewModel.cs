using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using GENAP_MAUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GENAP_MAUI.CustomViews.AlertDialogPopup
{
    public sealed partial class AlertDialogPopupViewModel : BaseViewModel
    {
        private readonly IPopupService _popupService;

        public AlertDialogPopupViewModel(IPopupService popupService)
        {
            _popupService = popupService;
        }

        [RelayCommand]
        public async Task Close() 
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }
    }
}
