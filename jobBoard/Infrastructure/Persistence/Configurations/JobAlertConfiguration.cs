using JobBoard.Domain.JobAlert;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class JobAlertConfiguration : IEntityTypeConfiguration<JobAlertEntity>
{
    public void Configure(EntityTypeBuilder<JobAlertEntity> builder)
    {
        builder.Property(ja => ja.EmailAddress)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(ja => ja.EmailAddress)
            .IsUnique();

        builder.Property(ja => ja.Keyword)
            .HasMaxLength(50);

        builder.HasOne(ja => ja.Category)
            .WithMany()
            .HasForeignKey(ja => ja.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ja => ja.Country)
            .WithMany()
            .HasForeignKey(ja => ja.CountryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ja => ja.EmploymentType)
            .WithMany()
            .HasForeignKey(ja => ja.EmploymentTypeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}