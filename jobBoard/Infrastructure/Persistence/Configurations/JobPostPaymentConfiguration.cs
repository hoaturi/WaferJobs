using JobBoard.Domain.JobPost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class JobPostPaymentConfiguration : IEntityTypeConfiguration<JobPostPaymentEntity>
{
    public void Configure(EntityTypeBuilder<JobPostPaymentEntity> builder)
    {
        builder.Property(jpp => jpp.CheckoutSessionId).HasMaxLength(255);
        builder.Property(jpp => jpp.EventId).HasMaxLength(255);
        builder.HasIndex(jpp => jpp.CheckoutSessionId).IsUnique();

        builder
            .HasOne(jpp => jpp.JobPost)
            .WithMany(jp => jp.Payments)
            .HasForeignKey(jpp => jpp.JobPostId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}