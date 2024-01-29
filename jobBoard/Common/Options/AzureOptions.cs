using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public record AzureOptions
{
    public const string Key = "Azure";

    [Required]
    public required string CommunicationServiceConnectionString { get; init; }

    [Required]
    public required string StorageConnectionString { get; init; }

    [Required]
    public required string BusinessLogoContainer { get; init; }

    [Required]
    public required string JobPostLogoContainer { get; init; }
}
