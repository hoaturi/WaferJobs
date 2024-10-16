using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaferJobs.Domain.Common;
using WaferJobs.Domain.JobAlert;
using WaferJobs.Domain.JobPost;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class JobAlertConfiguration : IEntityTypeConfiguration<JobAlertEntity>
{
    public void Configure(EntityTypeBuilder<JobAlertEntity> builder)
    {
        builder.Property(ja => ja.Email)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(ja => ja.Keyword)
            .HasMaxLength(100);

        builder.HasIndex(ja => ja.Email)
            .IsUnique();

        builder.HasIndex(ja => ja.Token)
            .IsUnique();

        builder.HasOne(ja => ja.Country)
            .WithMany()
            .HasForeignKey(ja => ja.CountryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(ja => ja.ExperienceLevels)
            .WithMany()
            .UsingEntity(
                "JobAlertExperienceLevels",
                r => r.HasOne(typeof(ExperienceLevelEntity)).WithMany().HasForeignKey("ExperienceLevelId"),
                l => l.HasOne(typeof(JobAlertEntity)).WithMany().HasForeignKey("JobAlertId")
            );

        builder
            .HasMany(ja => ja.EmploymentTypes)
            .WithMany()
            .UsingEntity(
                "JobAlertEmploymentTypes",
                r => r.HasOne(typeof(EmploymentTypeEntity)).WithMany().HasForeignKey("EmploymentTypeId"),
                l => l.HasOne(typeof(JobAlertEntity)).WithMany().HasForeignKey("JobAlertId")
            );

        builder
            .HasMany(ja => ja.Categories)
            .WithMany()
            .UsingEntity(
                "JobAlertCategories",
                r => r.HasOne(typeof(CategoryEntity)).WithMany().HasForeignKey("CategoryId"),
                l => l.HasOne(typeof(JobAlertEntity)).WithMany().HasForeignKey("JobAlertId")
            );
    }
}