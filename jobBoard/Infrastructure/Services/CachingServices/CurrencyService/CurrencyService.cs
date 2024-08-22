using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Options;
using JobBoard.Infrastructure.Persistence;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JobBoard.Infrastructure.Services.CachingServices.CurrencyService;

public class CurrencyService(
    AppDbContext dbContext,
    IDistributedCache cache,
    ILogger<CurrencyService> logger,
    IOptions<CurrencyOptions> options,
    HttpClient httpClient)
    : ICurrencyService
{
    private readonly CurrencyOptions _options = options.Value;

    public async Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRatesAsync(CancellationToken cancellationToken)
    {
        var cachedRates = await cache.GetAsync(CacheKeys.CurrencyExchangeRates, cancellationToken);

        if (cachedRates is not null)
            return MessagePackSerializer.Deserialize<IReadOnlyList<ExchangeRateDto>>(cachedRates,
                cancellationToken: cancellationToken);

        return await FetchAndCacheExchangeRatesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExchangeRateDto>> FetchAndCacheExchangeRatesAsync(
        CancellationToken cancellationToken)
    {
        var currencies = await dbContext.Currencies
            .ToListAsync(cancellationToken);

        var currencyCodes = string.Join(",", currencies.Select(c => c.Code));
        var queryStr = $"latest?base=USD&symbols={currencyCodes}&api_key={_options.ApiKey}";

        var response = await httpClient.GetAsync(queryStr, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var exchangeRates = JsonConvert.DeserializeObject<ExchangeRatesResponse>(content);

        if (exchangeRates?.Rates is not null)
        {
            foreach (var (code, rate) in exchangeRates.Rates)
            {
                var currency = currencies.FirstOrDefault(c => c.Code == code);
                if (currency is not null) currency.Rate = rate;

                await dbContext.SaveChangesAsync(cancellationToken);
            }

            var exchangeRateDtos = currencies.Select(c => new ExchangeRateDto(c.Id, c.Code, c.Rate)).ToList();

            var serializedRates =
                MessagePackSerializer.Serialize(exchangeRateDtos, cancellationToken: cancellationToken);

            await cache.SetAsync(CacheKeys.CurrencyExchangeRates, serializedRates, cancellationToken);

            logger.LogInformation("Updated currency exchange rates in cache and database");

            return exchangeRateDtos;
        }

        logger.LogError("Failed to deserialize currency API response");
        return [];
    }
}