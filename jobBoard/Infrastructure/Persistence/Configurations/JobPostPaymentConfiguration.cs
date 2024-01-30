using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard;

public class JobPostPaymentConfiguration : IEntityTypeConfiguration<JobPostPayment>
{
    public void Configure(EntityTypeBuilder<JobPostPayment> builder)
    {
        builder.Property(jpp => jpp.JobPostId).IsRequired();
        builder.Property(jpp => jpp.CheckoutSessionId).IsRequired().HasMaxLength(200);
        builder.Property(jpp => jpp.EventId).HasMaxLength(200);

        builder
            .HasOne(jpp => jpp.JobPost)
            .WithOne(jp => jp.Payment)
            .HasForeignKey<JobPostPayment>(jpp => jpp.JobPostId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
