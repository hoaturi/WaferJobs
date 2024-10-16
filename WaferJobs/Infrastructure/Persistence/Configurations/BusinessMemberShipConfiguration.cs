using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaferJobs.Domain.Business;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class BusinessMemberShipConfiguration : IEntityTypeConfiguration<BusinessMembershipEntity>
{
    public void Configure(EntityTypeBuilder<BusinessMembershipEntity> builder)
    {
        builder.Property(bm => bm.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(bm => bm.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(bm => bm.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(bm => bm.stripeCustomerId).HasMaxLength(255);

        builder
            .HasOne(bm => bm.User)
            .WithMany()
            .HasForeignKey(bm => bm.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}