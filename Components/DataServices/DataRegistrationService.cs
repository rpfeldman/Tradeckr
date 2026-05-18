using DomainModel;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataServices
{
    public sealed class DataRegistrationService(IStateStorage StateStorage)
    {
        private IStateStorage _StateStorage = StateStorage;

        public async Task<bool> RegistExpenseAsync(decimal value, DateOnly date, string category = "Uncategorized")
        {
            try
            {
                return await _StateStorage.SaveAsync(value, date, category, true, false, null);
            }
            catch (Exception)
            {
                return false;
            }
         
        }

        public async Task<bool> RegistExpenseAsync(decimal value, DateOnly date, int duration, string category = "Uncategorized")
        {
            try
            {
                return await _StateStorage.SaveAsync(value, date, category, true, true, duration);
            }
            catch (Exception)
            {
                return false;
            }
         
        }

        public async Task<bool> RegistIncomeAsync(decimal value, DateOnly date, string category = "Uncategorized")
        {
            try
            {
                return await _StateStorage.SaveAsync(value, date, category, false, false, null);
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> RegistIncomeAsync(decimal value, DateOnly date, int duration, string category = "Uncategorized")
        {
            try
            {
                return await _StateStorage.SaveAsync(value, date, category, false, true, duration);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
