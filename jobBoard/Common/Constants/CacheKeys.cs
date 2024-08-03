namespace JobBoard.Common.Constants;

public static class CacheKeys
{
    public const string RevokedToken = "revokedToken:";
    public const string CountriesCacheKey = "countries:WithJobPosts";
    public const string CitiesCacheKey = "cities:WithJobPosts";
    public const string LocationsCacheKey = "locations:WithJobPosts";
    public const string PopularKeywordsCacheKey = "popularKeywords";
    public const string JobCountCacheKey = "totalJobPostCount";
    public const string CurrencyExchangeRates = "CurrencyExchangeRates";
}