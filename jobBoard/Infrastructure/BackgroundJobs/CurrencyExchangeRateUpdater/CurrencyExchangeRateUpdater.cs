using JobBoard.Infrastructure.Services.LookupServices.CurrencyService;

namespace JobBoard.Infrastructure.BackgroundJobs.CurrencyExchangeRateUpdater;

public class CurrencyExchangeRateUpdater(
    ICurrencyService currencyService
) : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await currencyService.FetchAndCacheExchangeRatesAsync(cancellationToken);
    }
}