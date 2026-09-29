<<<<<<< HEAD
﻿using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GENAP_MAUI.ViewModels;
=======
﻿using GENAP_MAUI.ViewModels;
>>>>>>> a39a50c8291a2f57763f3c02720502a8c4fc39e8
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.CustomViews.DoubleActionDialogPopup
{
<<<<<<< HEAD
    public sealed partial class DoubleActionDialogPopupViewModel(IPopupService popupService) : BaseViewModel, IQueryAttributable
    {
        private readonly IPopupService _popupService = popupService;

        [ObservableProperty]
        public partial string Title { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Content { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string FirstOptionText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string SecondOptionText { get; set; } = string.Empty;

        [RelayCommand]
        public async Task FirstOption()
        {
            await _popupService.ClosePopupAsync(Shell.Current, 1);
        }
        
        [RelayCommand]
        public async Task SecondOption()
        {
            await _popupService.ClosePopupAsync(Shell.Current, 2);
        }
        
        [RelayCommand]
        public async Task Cancel()
        {
            await _popupService.ClosePopupAsync(Shell.Current, 0);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Title = (string)query[nameof(this.Title)];
            Content = (string)query[nameof(this.Content)];
            FirstOptionText = (string)query[nameof(this.FirstOptionText)];
            SecondOptionText = (string)query[nameof(this.SecondOptionText)];
        }
=======
    public sealed partial class DoubleActionDialogPopupViewModel : BaseViewModel
    {
>>>>>>> a39a50c8291a2f57763f3c02720502a8c4fc39e8
    }
}
