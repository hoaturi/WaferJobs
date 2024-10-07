using System.Net;
using JobBoard.Infrastructure.Options;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace JobBoard.Infrastructure.BackgroundJobs.CurrencyRateRefreshJob;

public class CurrencyRateRefreshJob(
    AppDbContext dbContext,
    ILogger<CurrencyRateRefreshJob> logger,
    IOptions<CurrencyOptions> options,
    HttpClient httpClient)
    : IRecurringJobBase
{
    private readonly CurrencyOptions _options = options.Value;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching and updating exchange rates.");

        var currencies = await dbContext.Currencies.ToListAsync(cancellationToken);

        var currencyCodes = string.Join(",", currencies.Select(c => c.Code));
        var exchangeRateApiUrl = $"latest?base=USD&symbols={currencyCodes}&api_key={_options.ApiKey}";

        var response = await httpClient.GetAsync(exchangeRateApiUrl, cancellationToken);

        if (!response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
        {
            logger.LogWarning("Failed to fetch exchange rates. Status code: {StatusCode}", response.StatusCode);
            return;
        }

        var responseContent = await response.Content.ReadFromJsonAsync<CurrencyRatesResponse>(cancellationToken);

        if (responseContent?.Rates is not null)
        {
            foreach (var currency in currencies)
            {
                if (!responseContent.Rates.TryGetValue(currency.Code, out var newRate)) continue;
                currency.Rate = newRate;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Updated exchange rates for {CurrencyCount} currencies.", currencies.Count);
        }
    }
}