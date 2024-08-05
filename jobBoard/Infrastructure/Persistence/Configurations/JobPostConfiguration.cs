using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class JobPostConfiguration : IEntityTypeConfiguration<JobPostEntity>
{
    public void Configure(EntityTypeBuilder<JobPostEntity> builder)
    {
        builder.Property(jp => jp.Title).IsRequired().HasMaxLength(100);
        builder.Property(jp => jp.Description).IsRequired().HasMaxLength(10000);
        builder.Property(jp => jp.ApplyUrl).HasMaxLength(2000);
        builder.Property(jp => jp.CompanyName).IsRequired().HasMaxLength(50);
        builder.Property(jp => jp.CompanyLogoUrl).HasMaxLength(2000);
        builder.Property(jp => jp.CompanyWebsiteUrl).HasMaxLength(2000);

        builder.HasIndex(jp => jp.IsPublished);
        builder.HasIndex(jp => jp.IsDeleted);
        builder.HasIndex(jp => jp.PublishedAt);
        builder.HasIndex(jp => jp.IsFeatured);
        builder.HasIndex(jp => jp.IsPublished);
        builder.HasIndex(jp => jp.MinSalary);
        builder.HasIndex(jp => jp.MaxSalary);


        builder.HasGeneratedTsVectorColumn(ja => ja.SearchVector, "english",
                ja => new { ja.Title, ja.Description, ja.CompanyName })
            .HasIndex("SearchVector")
            .HasMethod("GIN");

        builder.HasOne(jp => jp.City)
            .WithMany()
            .HasForeignKey(jp => jp.CityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(jp => jp.Currency)
            .WithMany()
            .HasForeignKey(jp => jp.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(jp => jp.ExperienceLevel)
            .WithMany()
            .HasForeignKey(jp => jp.ExperienceLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(jp => jp.Country)
            .WithMany()
            .HasForeignKey(jp => jp.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(jp => jp.Category)
            .WithMany()
            .HasForeignKey(jp => jp.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(jp => jp.EmploymentType)
            .WithMany()
            .HasForeignKey(jp => jp.EmploymentTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(jp => jp.Tags)
            .WithMany()
            .UsingEntity(
                "JobPostTags",
                r => r.HasOne(typeof(TagEntity)).WithMany().HasForeignKey("TagId"),
                l => l.HasOne(typeof(JobPostEntity)).WithMany().HasForeignKey("JobPostId")
            );
    }
}