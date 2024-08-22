namespace JobBoard.Infrastructure.Services.CachingServices.CurrencyService;

public interface ICurrencyService
{
    Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRatesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ExchangeRateDto>> FetchAndCacheExchangeRatesAsync(CancellationToken cancellationToken);
}