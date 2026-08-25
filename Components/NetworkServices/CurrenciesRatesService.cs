using System;
using System.Collections.Generic;
using System.Text.Json;
using DomainModel;
using System.Text.Json.Serialization;
using System.Reflection.Metadata;
using System.Net.Mime;

namespace NetworkServices
{
    public sealed class CurrenciesRatesService(string rootCurrencyIsoCode)
    {
        private string _RootCurrencyIsoCode = rootCurrencyIsoCode;
        private HttpClient _httpClient = new();

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
    }

    sealed class CurrencyJsonDto
    {
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = [];
    }
}
