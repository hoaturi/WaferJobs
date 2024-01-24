using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public record DbConnectionOptions
{
    public const string Key = "ConnectionStrings";

    [Required]
    public required string JobBoardApiDb { get; init; }
}
