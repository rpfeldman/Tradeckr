using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Services;
using GENAP_MAUI.CustomViews.AlertDialogPopup;
using GENAP_MAUI.CustomViews.DoubleActionDialogPopup;
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

            var popupOptions = new PopupOptions
            {
                PageOverlayColor = Color.FromArgb("#D90B0716"), 
                Shape = null,                                    
                Shadow = null                                    
            };

            await _popupService.ShowPopupAsync<AlertDialogPopupViewModel>(
                Shell.Current, 
                options: popupOptions,
                shellParameters: queryAttributes
                );
        }

        public async Task<int> ShowDoubleActionDialogAsync(string Title, string Content, string FirstOption, string SecondOption)
        {
            var queryAttributes = new Dictionary<string, object>
            {
                 [nameof(DoubleActionDialogPopupViewModel.Title)] = Title,
                 [nameof(DoubleActionDialogPopupViewModel.Content)] = Content,
                 [nameof(DoubleActionDialogPopupViewModel.FirstOptionText)] = FirstOption,
                 [nameof(DoubleActionDialogPopupViewModel.SecondOptionText)] = SecondOption,
            };

            var popupOptions = new PopupOptions
            {
                PageOverlayColor = Color.FromArgb("#D90B0716"), 
                Shape = null,                                    
                Shadow = null                                    
            };

            var result = await _popupService.ShowPopupAsync<DoubleActionDialogPopupViewModel>(
                Shell.Current, 
                options: popupOptions,
                shellParameters: queryAttributes);

            if (result.WasDismissedByTappingOutsideOfPopup) { return 0; }

            return ((IPopupResult<int>)result).Result;
        }
    }
}
