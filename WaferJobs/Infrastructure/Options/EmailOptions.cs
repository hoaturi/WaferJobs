using System.ComponentModel.DataAnnotations;

namespace WaferJobs.Infrastructure.Options;

public class EmailOptions
{
    public const string Key = "Email";

    [Required] public required string SenderEmail { get; init; }
    [Required] public required string SenderName { get; init; }
    [Required] public required string BaseUrl { get; init; }
}