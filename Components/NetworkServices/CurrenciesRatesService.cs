using System;
using System.Collections.Generic;
using System.Text.Json;
using DomainModel;
using System.Text.Json.Serialization;
using System.Reflection.Metadata;
using System.Net.Mime;
using System.Security;

namespace NetworkServices
{
    public sealed class CurrenciesRatesService(string rootCurrencyIsoCode)
    {
        private string _RootCurrencyIsoCode = rootCurrencyIsoCode;
        private HttpClient _httpClient = new();

        //trycatch pending
        public async Task<OperationResult<Dictionary<string, decimal>>> GetRatesAsync(DateOnly date)
        {
            if (!NetworkMethods.CheckInternetConnection())
            {
                return OperationResult<Dictionary<string, decimal>>.FaultedOperation(new InnerErrorDto()); // TO-DO
            }

            var formattedDate = date.ToString("yyyy-MM-dd");

            var request = await _httpClient.GetAsync($"https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@{formattedDate}/v1/currencies/{_RootCurrencyIsoCode}.json");

            if (!request.IsSuccessStatusCode)
            {
                return OperationResult<Dictionary<string, decimal>>.FaultedOperation(new InnerErrorDto()); // TO-DO
            }

            var content = await request.Content.ReadAsStringAsync();

            content = content.Substring(0, content.IndexOf(_RootCurrencyIsoCode)) + "Rates" + content.Substring(content.IndexOf(_RootCurrencyIsoCode) + _RootCurrencyIsoCode.Length); // we replace the isocode of the JSON text to 'Rates' to match the property

            var result = JsonSerializer.Deserialize<CurrencyJsonDto>(content)?.Rates;
            return OperationResult<Dictionary<string, decimal>>.SuccessfulOperation(result!);
        }

        public async Task<OperationResult> UpdateCurrenciesRate(CurrencyDto[] currencies, DateOnly date)
        {
            var getRatesOperation = await GetRatesAsync(date);

            if(!getRatesOperation.Success)
            {
                return OperationResult.FaultedOperation(getRatesOperation.InnerError);
            }

            var rates = getRatesOperation.Result!;

            for (int i = 0; i < currencies.Length; i++)
            {
                if (!rates.TryGetValue(currencies[i].IsoCode, out decimal value))
                {
                    return OperationResult.FaultedOperation(new InnerErrorDto()); // TO-DO
                }

                var updatedCurrency = new CurrencyDto() { CurrencyDisplayName =  currencies[i].CurrencyDisplayName, IsoCode = currencies[i].IsoCode, ConversionRate = value };
                currencies[i] = updatedCurrency; 
            }

            return OperationResult.SuccessfulOperation();
        }
    }

    sealed class CurrencyJsonDto
    {
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = [];
    }
}
