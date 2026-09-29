using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Services;
using GENAP_MAUI.CustomViews.AlertDialogPopup;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.CustomViews
{
    public sealed class CustomDialogsService
    {
        private readonly IPopupService _popupService;

        public CustomDialogsService(IPopupService popupService)
        {
            _popupService = popupService;
        }

        public async Task ShowAlertDialogAsync(string Title, string Content, string ButtonText)
        {
             var queryAttributes = new Dictionary<string, object>
            {
                 [nameof(AlertDialogPopupViewModel.Title)] = Title,
                 [nameof(AlertDialogPopupViewModel.Content)] = Content,
                 [nameof(AlertDialogPopupViewModel.ButtonText)] = ButtonText,
            };

            await _popupService.ShowPopupAsync<AlertDialogPopupViewModel>(
                Shell.Current, 
                options: PopupOptions.Empty,
                shellParameters: queryAttributes
                );
        }
    }
}
