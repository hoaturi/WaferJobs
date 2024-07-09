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
        builder.Property(jp => jp.Currency).HasMaxLength(3);
        builder.Property(jp => jp.ApplyUrl).HasMaxLength(2000);
        builder.Property(jp => jp.CompanyName).IsRequired().HasMaxLength(50);
        builder.Property(jp => jp.CompanyLogoUrl).HasMaxLength(2000);
        builder.Property(jp => jp.CompanyWebsiteUrl).HasMaxLength(2000);

        builder.HasIndex(jp => jp.IsPublished);
        builder.HasIndex(jp => jp.IsDeleted);
        builder.HasIndex(jp => jp.PublishedAt);


        builder.HasIndex(jp => jp.Title).HasMethod("GIN").HasOperators("gin_trgm_ops");
        builder.HasIndex(jp => jp.Description).HasMethod("GIN").HasOperators("gin_trgm_ops");
        builder.HasIndex(jp => jp.CompanyName).HasMethod("GIN").HasOperators("gin_trgm_ops");
        builder.HasIndex(jp => jp.Tags).HasMethod("GIN");

        builder.HasOne(jp => jp.City)
            .WithMany()
            .HasForeignKey(jp => jp.CityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(jp => jp.Country)
            .WithMany()
            .HasForeignKey(jp => jp.CountryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}