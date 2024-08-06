namespace JobBoard.Infrastructure.Services.LookupServices.CurrencyService;

public interface ICurrencyService
{
    Task<IReadOnlyList<ExchangeRateDto>> GetExchangeRatesAsync(CancellationToken cancellationToken);
}