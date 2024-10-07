namespace JobBoard.Infrastructure.BackgroundJobs.CurrencyRateRefreshJob;

public record CurrencyRatesResponse(
    Dictionary<string, decimal> Rates
);