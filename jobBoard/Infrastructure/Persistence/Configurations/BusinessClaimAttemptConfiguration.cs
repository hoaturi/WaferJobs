using JobBoard.Domain.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class BusinessClaimAttemptConfiguration : IEntityTypeConfiguration<BusinessClaimAttemptEntity>
{
    public void Configure(EntityTypeBuilder<BusinessClaimAttemptEntity> builder)
    {
        builder.Property(bca => bca.ClaimantEmail)
            .HasMaxLength(254)
            .IsRequired();

        builder.Property(bca => bca.ClaimantFirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(bca => bca.ClaimantLastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(bca => bca.ClaimantTitle)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(bca => bca.Notes)
            .HasMaxLength(500);

        builder.HasOne(bca => bca.ClaimantUser)
            .WithMany()
            .HasForeignKey(bca => bca.ClaimantUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(bca => bca.Verifier)
            .WithMany()
            .HasForeignKey(bca => bca.VerifierId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}