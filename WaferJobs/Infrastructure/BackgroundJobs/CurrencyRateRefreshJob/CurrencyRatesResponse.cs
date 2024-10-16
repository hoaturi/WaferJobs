namespace WaferJobs.Infrastructure.BackgroundJobs.CurrencyRateRefreshJob;

public record CurrencyRatesResponse(
    Dictionary<string, decimal> Rates
);