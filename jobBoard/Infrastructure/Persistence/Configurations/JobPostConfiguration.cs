using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard;

public class JobPostConfiguration : IEntityTypeConfiguration<JobPost>
{
    public void Configure(EntityTypeBuilder<JobPost> builder)
    {
        builder.Property(jp => jp.Title).IsRequired().HasMaxLength(100);
        builder.Property(jp => jp.Description).IsRequired().HasMaxLength(10000);
        builder.Property(jp => jp.City).HasMaxLength(50);
        builder.Property(jp => jp.Currency).HasMaxLength(3);
        builder.Property(jp => jp.ApplyUrl).HasMaxLength(2000);
        builder.Property(jp => jp.CompanyName).IsRequired().HasMaxLength(50);
        builder.Property(jp => jp.CompanyLogoUrl).HasMaxLength(2000);
    }
}
