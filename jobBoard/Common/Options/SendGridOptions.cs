using System.ComponentModel.DataAnnotations;

namespace JobBoard.Common.Options;

public class SendGridOptions
{
    public const string Key = "SendGrid";

    [Required] public required string ApiKey { get; init; }
    [Required] public required string JobAlertTemplateId { get; init; }
}