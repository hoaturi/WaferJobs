using System.ComponentModel.DataAnnotations;

namespace JobBoard.Infrastructure.Options;

public record DbConnectionOptions
{
    public const string Key = "ConnectionStrings";
    [Required] public required string WaferJobsDb { get; init; }
}