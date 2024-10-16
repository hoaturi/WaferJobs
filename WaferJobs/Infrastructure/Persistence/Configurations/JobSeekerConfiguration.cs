using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaferJobs.Domain.JobSeeker;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class JobSeekerConfiguration : IEntityTypeConfiguration<JobSeekerEntity>
{
    public void Configure(EntityTypeBuilder<JobSeekerEntity> builder)
    {
        builder.Property(j => j.Name).IsRequired().HasMaxLength(50);

        builder.HasOne(j => j.User)
            .WithOne()
            .HasForeignKey<JobSeekerEntity>(j => j.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}