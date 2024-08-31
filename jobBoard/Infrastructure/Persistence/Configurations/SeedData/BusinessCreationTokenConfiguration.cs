using JobBoard.Domain.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations.SeedData;

public class BusinessCreationTokenConfiguration : IEntityTypeConfiguration<BusinessCreationTokenEntity>
{
    public void Configure(EntityTypeBuilder<BusinessCreationTokenEntity> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.WebsiteUrl).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.Domain).HasMaxLength(2048).IsRequired();

        builder.HasIndex(x => x.Token);
        builder.HasIndex(x => x.ExpiresAt);
        builder.HasIndex(x => x.IsUsed);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}