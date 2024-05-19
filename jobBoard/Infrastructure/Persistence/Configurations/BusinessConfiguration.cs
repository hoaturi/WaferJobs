using JobBoard.Domain.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<BusinessEntity>
{
    public void Configure(EntityTypeBuilder<BusinessEntity> builder)
    {
        builder.Property(b => b.Name).IsRequired().HasMaxLength(50);
        builder.Property(b => b.LogoUrl).HasMaxLength(500);
        builder.Property(b => b.Description).HasMaxLength(5000);
        builder.Property(b => b.Location).HasMaxLength(50);
        builder.Property(b => b.StripeCustomerId).HasMaxLength(50);
        builder.Property(b => b.Url).HasMaxLength(500);
        builder.Property(b => b.TwitterUrl).HasMaxLength(500);
        builder.Property(b => b.LinkedInUrl).HasMaxLength(500);
        builder.Property(b => b.UserId).IsRequired();

        builder
            .HasMany(b => b.JobPosts)
            .WithOne(jp => jp.Business)
            .HasForeignKey(jp => jp.BusinessId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}