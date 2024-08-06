using MessagePack;

namespace JobBoard.Infrastructure.Services.LookupServices.CurrencyService;

[MessagePackObject]
public record ExchangeRateDto(
    [property: Key(0)] int Id,
    [property: Key(1)] string Code,
    [property: Key(2)] decimal Rate
);