using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public record AzureOptions
{
    public const string Key = "Azure";

    [Required]
    public required string CommunicationServiceConnectionStrings { get; init; }
}
