using JobBoard.Domain.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class BusinessMemberInvitationConfiguration : IEntityTypeConfiguration<BusinessMemberInvitationEntity>
{
    public void Configure(EntityTypeBuilder<BusinessMemberInvitationEntity> builder)
    {
        builder.Property(b => b.InviteeEmail)
            .HasMaxLength(254);

        builder.HasIndex(b => b.Token)
            .IsUnique();

        builder.HasIndex(b => new { b.BusinessId, b.InviteeEmail })
            .IsUnique();

        builder.HasOne(b => b.Business)
            .WithMany()
            .HasForeignKey(b => b.BusinessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Inviter)
            .WithMany()
            .HasForeignKey(b => b.InviterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}