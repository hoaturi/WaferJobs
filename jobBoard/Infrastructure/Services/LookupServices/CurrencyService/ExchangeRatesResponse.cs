using System.Text.Json.Serialization;

namespace JobBoard.Infrastructure.Services.LookupServices.CurrencyService;

public record ExchangeRatesResponse(
    [property: JsonPropertyName("rates")] Dictionary<string, decimal> Rates
);