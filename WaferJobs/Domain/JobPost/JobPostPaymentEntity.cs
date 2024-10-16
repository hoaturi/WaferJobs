using WaferJobs.Common;

namespace WaferJobs.Domain.JobPost;

public class JobPostPaymentEntity : BaseEntity
{
    public Guid Id { get; init; }
    public Guid JobPostId { get; init; }
    public required string CheckoutSessionId { get; init; }
    public string? EventId { get; set; }
    public bool IsProcessed { get; set; }

    public JobPostEntity? JobPost { get; init; }
}