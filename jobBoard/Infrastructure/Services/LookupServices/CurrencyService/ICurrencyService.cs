using JobBoard.Domain.Common;

namespace JobBoard.Infrastructure.Services.LookupServices.CurrencyService;

public interface ICurrencyService
{
    Task<IReadOnlyList<CurrencyEntity>> GetExchangeRatesAsync(CancellationToken cancellationToken);
}