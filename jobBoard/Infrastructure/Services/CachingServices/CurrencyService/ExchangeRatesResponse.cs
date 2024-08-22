using System.Text.Json.Serialization;

namespace JobBoard.Infrastructure.Services.CachingServices.CurrencyService;

public record ExchangeRatesResponse(
    [property: JsonPropertyName("rates")] Dictionary<string, decimal> Rates
);