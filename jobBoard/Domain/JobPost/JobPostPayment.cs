namespace JobBoard;

public class JobPostPayment : BaseEntity
{
    public Guid Id { get; set; }
    public Guid JobPostId { get; set; }
    public required string CheckoutSessionId { get; set; }
    public string? EventId { get; set; }
    public bool IsProcessed { get; set; } = false;

    public JobPost JobPost { get; set; } = null!;
}
