using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaferJobs.Domain.Conference;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class ConferenceConfiguration : IEntityTypeConfiguration<ConferenceEntity>
{
    public void Configure(EntityTypeBuilder<ConferenceEntity> builder)
    {
        builder.Property(c => c.ContactEmail).HasMaxLength(254);
        builder.Property(c => c.ContactName).HasMaxLength(50);
        builder.Property(c => c.Title).HasMaxLength(150);
        builder.Property(c => c.Organiser).HasMaxLength(150);
        builder.Property(c => c.OrganiserEmail).HasMaxLength(254);
        builder.Property(c => c.Location).HasMaxLength(150);
        builder.Property(c => c.WebsiteUrl).HasMaxLength(2048);

        builder.HasIndex(c => c.StartDate);
        builder.HasIndex(c => c.IsVerified);
        builder.HasIndex(c => c.IsPublished);
    }
}