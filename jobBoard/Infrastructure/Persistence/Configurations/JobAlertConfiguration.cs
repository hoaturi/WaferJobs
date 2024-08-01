using JobBoard.Domain.JobAlert;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class JobAlertConfiguration : IEntityTypeConfiguration<JobAlertEntity>
{
    public void Configure(EntityTypeBuilder<JobAlertEntity> builder)
    {
        builder.Property(ja => ja.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(ja => ja.Email)
            .IsUnique();

        builder.HasIndex(ja => ja.Token)
            .IsUnique();

        builder.Property(ja => ja.Keyword)
            .HasMaxLength(50);

        builder.HasOne(ja => ja.Country)
            .WithMany()
            .HasForeignKey(ja => ja.CountryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(ja => ja.EmploymentTypes)
            .WithMany(et => et.JobAlerts)
            .UsingEntity("JobAlertEmploymentType");

        builder
            .HasMany(ja => ja.Categories)
            .WithMany(c => c.JobAlerts)
            .UsingEntity("JobAlertCategory")
            ;
    }
}