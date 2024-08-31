using JobBoard.Domain.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class BusinessClaimTokenConfiguration : IEntityTypeConfiguration<BusinessClaimTokenEntity>
{
    public void Configure(EntityTypeBuilder<BusinessClaimTokenEntity> builder)
    {
        builder.HasIndex(bca => bca.ExpiresAt);
        builder.HasIndex(bca => bca.IsUsed);
        builder.HasIndex(bca => bca.Token);

        builder.HasOne(bca => bca.Business)
            .WithMany()
            .HasForeignKey(bca => bca.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bca => bca.User)
            .WithMany()
            .HasForeignKey(bca => bca.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}