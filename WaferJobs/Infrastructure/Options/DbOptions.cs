using System.ComponentModel.DataAnnotations;

namespace WaferJobs.Infrastructure.Options;

public record DbOptions
{
    public const string Key = "Db";
    [Required] public required string WaferJobsDb { get; init; }
}