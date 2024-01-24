using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
    public void Configure(EntityTypeBuilder<Business> builder)
    {
        builder.Property(b => b.Name).IsRequired().HasMaxLength(50);
        builder.Property(b => b.LogoUrl).HasMaxLength(500);
        builder.Property(b => b.Description).HasMaxLength(5000);
        builder.Property(b => b.Location).HasMaxLength(50);
        builder.Property(b => b.Size).HasMaxLength(20);
        builder.Property(b => b.StripeCustomerId).HasMaxLength(50);
        builder.Property(b => b.Url).HasMaxLength(500);
        builder.Property(b => b.TwitterUrl).HasMaxLength(500);
        builder.Property(b => b.LinkedInUrl).HasMaxLength(500);
        builder.Property(b => b.UserId).IsRequired();
    }
}
