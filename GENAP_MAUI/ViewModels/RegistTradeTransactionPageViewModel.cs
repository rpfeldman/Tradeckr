using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServices;
using DomainModel;
using GENAP_MAUI.CustomViews;
using GENAP_MAUI.InnerComponents;
using GENAP_MAUI.Resources.Strings;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace GENAP_MAUI.ViewModels
{
    public sealed partial class RegistTradeTransactionPageViewModel(DataRegistrationService dataRegistrationService, CustomDialogsService customDialogsService) : BaseViewModel
    {
        private readonly DataRegistrationService _RegistrationService = dataRegistrationService;
        private readonly CustomDialogsService _customDialogsService = customDialogsService;

        private bool IsAnAlert = false;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial decimal Value { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegistTransactionCommand))]
        public partial string PickedValue { get; set; }

        [ObservableProperty]
        public partial DateTime PickedDate { get; set; }

        [ObservableProperty]
        public partial CurrencyDto PickedCurrency { get; set; }

        [ObservableProperty]
        public partial bool Depletion { get; set; }
        public bool IsIncomeSelected => !Depletion;
        public bool IsExpenseSelected => Depletion;

        partial void OnDepletionChanged(bool value)
        {
            OnPropertyChanged(nameof(IsIncomeSelected));
            OnPropertyChanged(nameof(IsExpenseSelected));
        }

        partial void OnPickedValueChanged(string value)
        {
            if (decimal.TryParse(value, out decimal newValue))
            {
                Value = newValue;
                return;
            }
        
            Value = 0m;
        }

        [RelayCommand(CanExecute = nameof(RegistTransactionCanExecute))]
        public async Task RegistTransaction()
        {
            Value = CurrencyConverterService.CurrencyToTfu(Value, PickedCurrency);  

            var alertContents = AppResources.TransactionRegistered_AlertContent.Split('_');

            if (Depletion)
            {
                var ExpenseRegistrationOperation = await _RegistrationService.RegistExpenseAsync(Value, DateOnly.FromDateTime(PickedDate), DefaultCategories.TradingCategoryName);
                    ExpenseRegistrationOperation.WriteLog
                        (
                            $"Save new trading loss in the storage\n" +
                            $" -Intended value: '{(CurrencyConverterService.TfuToCurrency(Value, PickedCurrency)):N2} {PickedCurrency.IsoCode}$'\n" +
                            $" -Real value: '{Value:N4} {GlobalResources.DefaultTradingCurrency.IsoCode}$'\n" +
                            $" -Date: '{PickedDate:dd/MM/yyyy}'"
                        );


                    IsAnAlert = true;

                await _customDialogsService.ShowAlertDialogAsync
                    (
                        AppResources.TransactionRegistered_AlertTitle, 
                        ExpenseRegistrationOperation.Success ? alertContents[2] : ExpenseRegistrationOperation.InnerError?.ErrorMessage!, 
                        AppResources.Accept_AlertButton
                    );

                    IsAnAlert = false;

                return;
            }

            var IncomeRegistrationOperation = await _RegistrationService.RegistIncomeAsync(Value, DateOnly.FromDateTime(PickedDate), DefaultCategories.TradingCategoryName);
                IncomeRegistrationOperation.WriteLog
                    (
                        $"Save new trading win in the storage\n" +
                        $" -Intended value: '{(CurrencyConverterService.TfuToCurrency(Value, PickedCurrency)):N2} {PickedCurrency.IsoCode}$'\n" +
                        $" -Real value: '{Value:N4} {GlobalResources.DefaultTradingCurrency.IsoCode}$'\n" +
                        $" -Date: '{PickedDate:dd/MM/yyyy}'"
                    );    

                IsAnAlert = true;

            await _customDialogsService.ShowAlertDialogAsync
                (
                    AppResources.TransactionRegistered_AlertTitle,
                    IncomeRegistrationOperation.Success ? alertContents[3] : IncomeRegistrationOperation.InnerError?.ErrorMessage!,
                    AppResources.Accept_AlertButton
                );

                IsAnAlert = false;

            return;
        }

        [RelayCommand]
        public void ReLoad()
        {
            if(IsAnAlert) return;

            Value = 0m;
            PickedValue = string.Empty;
            PickedDate = DateTime.Today;
            PickedCurrency = GlobalResources.DefaultTradingCurrency;
            Depletion = true;
        }

        [RelayCommand] void SetIncome() => Depletion = false;
        [RelayCommand] void SetExpense() => Depletion = true;
        private bool RegistTransactionCanExecute() => Value > 0m && Value <= 1000000000m;
    }
}
