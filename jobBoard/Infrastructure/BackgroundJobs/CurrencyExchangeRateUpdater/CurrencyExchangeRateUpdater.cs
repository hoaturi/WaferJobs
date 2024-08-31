using JobBoard.Infrastructure.Options;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JobBoard.Infrastructure.BackgroundJobs.CurrencyExchangeRateUpdater;

public class CurrencyExchangeRateUpdater(
    AppDbContext dbContext,
    ILogger<CurrencyExchangeRateUpdater> logger,
    IOptions<CurrencyOptions> options,
    HttpClient httpClient)
    : IRecurringJobBase
{
    private readonly CurrencyOptions _options = options.Value;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await FetchAndUpdateExchangeRatesAsync(cancellationToken);
    }

    private async Task FetchAndUpdateExchangeRatesAsync(CancellationToken cancellationToken)
    {
        var currencies = await dbContext.Currencies.ToListAsync(cancellationToken);

        var currencyCodes = string.Join(",", currencies.Select(c => c.Code));
        var exchangeRateApiUrl = $"latest?base=USD&symbols={currencyCodes}&api_key={_options.ApiKey}";

        var response = await httpClient.GetFromJsonAsync<ExchangeRatesResponse>(exchangeRateApiUrl, cancellationToken);

        if (response?.Rates is not null)
        {
            foreach (var currency in currencies)
            {
                if (!response.Rates.TryGetValue(currency.Code, out var newRate)) continue;
                currency.Rate = newRate;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Currency exchange rates updated successfully");
        }
        else
        {
            logger.LogError("Failed to retrieve or deserialize currency API response");
        }
    }
}