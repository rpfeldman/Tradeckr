using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class RegistTransactionPageViewModel(GlobalResources globalResources) : BaseViewModel
    {
        public GlobalResources GR = globalResources;

        [ObservableProperty]
        public partial string Category { get; set; } = string.Empty;

        [ObservableProperty]
        public partial DateTime PickedDate { get; set; } = DateTime.Today;

        [ObservableProperty]
        public partial bool Depletion { get; set; } = true;

        [RelayCommand]
        public void ChangeDepletion()
        {
            Depletion = Depletion ? false : true;
        }
    }
}
