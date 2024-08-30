using JobBoard.Domain.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class BusinessClaimRequestConfiguration : IEntityTypeConfiguration<BusinessClaimRequestEntity>
{
    public void Configure(EntityTypeBuilder<BusinessClaimRequestEntity> builder)
    {
        builder.HasIndex(bca => bca.Pin)
            .IsUnique();

        builder.HasIndex(bca => bca.ExpiresAt);
        builder.HasIndex(bca => bca.IsVerified);

        builder.HasOne(bca => bca.Business)
            .WithMany()
            .HasForeignKey(bca => bca.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bca => bca.ClaimantUser)
            .WithMany()
            .HasForeignKey(bca => bca.ClaimantUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}