﻿using JobBoard.Domain.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class BusinessMemberConfiguration : IEntityTypeConfiguration<BusinessMemberEntity>
{
    public void Configure(EntityTypeBuilder<BusinessMemberEntity> builder)
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
            .HasOne(bm => bm.Business)
            .WithMany(b => b.Members)
            .HasForeignKey(bm => bm.BusinessId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(bm => bm.User)
            .WithMany()
            .HasForeignKey(bm => bm.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}