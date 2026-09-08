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
    // I want to thank 'fawazahmed0' for making this free and open source exchange api.
    // Here's the link to the official repo: https://github.com/fawazahmed0/exchange-api
    public sealed class CurrenciesRatesService
    {
        private string _RootCurrencyIsoCode;
        private HttpClient _httpClient;

        public CurrenciesRatesService(string rootCurrencyIsoCode)
        {
            _RootCurrencyIsoCode = rootCurrencyIsoCode.ToLower();

            _httpClient = new()
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
        }

        public async Task<OperationResult<Dictionary<string, decimal>>> GetRatesAsync(DateOnly date)
        {
            try
            {
                if (date > DateOnly.FromDateTime(DateTime.Today) || date < new DateOnly(2024, 3, 2))
                {
                    throw new ArgumentException($"{nameof(date)} cannot be in the future or before March 2, 2024");
                }

                if (!NetworkMethods.CheckInternetConnection())
                {
                    return OperationResult<Dictionary<string, decimal>>.FaultedOperation(NetworkErrors.InternetConnectionError); 
                }

                var formattedDate = date.ToString("yyyy-MM-dd");

                var request = await _httpClient.GetAsync($"https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@{formattedDate}/v1/currencies/{_RootCurrencyIsoCode}.json");

                if (!request.IsSuccessStatusCode)
                {
                    return OperationResult<Dictionary<string, decimal>>.FaultedOperation(NetworkErrors.HttpStatusCodeError(request.StatusCode)); 
                }

                var content = await request.Content.ReadAsStringAsync();

                content = content.Substring(0, content.IndexOf(_RootCurrencyIsoCode)) + "Rates" + content.Substring(content.IndexOf(_RootCurrencyIsoCode) + _RootCurrencyIsoCode.Length); // we replace the isocode of the JSON text to 'Rates' to match the property

                var result = JsonSerializer.Deserialize<CurrencyJsonDto>(content)?.Rates;
                return OperationResult<Dictionary<string, decimal>>.SuccessfulOperation(result!);

            }
            catch (HttpRequestException x)
            {
                return OperationResult<Dictionary<string, decimal>>.FaultedOperation(NetworkErrors.HttpRequestError(x)); 
            }
            catch (TaskCanceledException x)
            {
                return OperationResult<Dictionary<string, decimal>>.FaultedOperation(NetworkErrors.TimeoutError(x)); 
            }
            catch (JsonException x)
            {
                return OperationResult<Dictionary<string, decimal>>.FaultedOperation(NetworkErrors.JsonError(x)); 
            }
        }

        public async Task<OperationResult<int>> UpdateCurrenciesRatesAsync(CurrencyDto[] currencies, DateOnly date)
        {
            if(currencies is null)
            {
                throw new ArgumentNullException(nameof(currencies));
            }

            int invalidCurrencies = 0;
            var getRatesOperation = await GetRatesAsync(date);

            if (!getRatesOperation.Success)
            {
                return OperationResult<int>.FaultedOperation(getRatesOperation.InnerError);
            }

            var rates = getRatesOperation.Result!;

            for (int i = 0; i < currencies.Length; i++)
            {
                if (!rates.TryGetValue(currencies[i].IsoCode.ToLower(), out decimal value))
                {
                    invalidCurrencies++;
                    continue;
                }

                var updatedCurrency = new CurrencyDto() { CurrencyDisplayName = currencies[i].CurrencyDisplayName, IsoCode = currencies[i].IsoCode, Id = currencies[i].Id, ConversionRate = value };
                currencies[i] = updatedCurrency;
            }

            return OperationResult<int>.SuccessfulOperation(currencies.Length - invalidCurrencies);
        }
    }

    sealed class CurrencyJsonDto
    {
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = [];
    }
}
