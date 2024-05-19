using System.ComponentModel.DataAnnotations;

namespace JobBoard.Common.Options;

public record DbConnectionOptions
{
    public const string Key = "ConnectionStrings";

    [Required] public required string JobBoardApiDb { get; init; }
}