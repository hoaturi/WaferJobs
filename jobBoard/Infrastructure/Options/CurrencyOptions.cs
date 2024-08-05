using System.ComponentModel.DataAnnotations;

namespace JobBoard.Infrastructure.Options;

public class CurrencyOptions
{
    public const string Key = "Currency";

    [Required] public required string ApiKey { get; set; }
    [Required] public required string BaseUrl { get; set; }
}