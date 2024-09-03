using JobBoard.Domain.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class BusinessMemberConfiguration : IEntityTypeConfiguration<BusinessMembershipEntity>
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

        builder
            .HasOne(bm => bm.User)
            .WithMany()
            .HasForeignKey(bm => bm.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}