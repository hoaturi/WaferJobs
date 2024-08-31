namespace JobBoard.Infrastructure.BackgroundJobs.CurrencyExchangeRateUpdater;

public record ExchangeRatesResponse(
    Dictionary<string, decimal> Rates
);