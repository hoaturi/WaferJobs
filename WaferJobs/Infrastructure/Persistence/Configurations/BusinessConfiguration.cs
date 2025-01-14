﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaferJobs.Domain.Business;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class BusinessConfiguration : IEntityTypeConfiguration<BusinessEntity>
{
    public void Configure(EntityTypeBuilder<BusinessEntity> builder)
    {
        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.LogoUrl).HasMaxLength(2048);
        builder.Property(b => b.Description).HasMaxLength(2000);
        builder.Property(b => b.Location).HasMaxLength(100);
        builder.Property(b => b.WebsiteUrl).HasMaxLength(2048);
        builder.Property(b => b.TwitterUrl).HasMaxLength(2048);
        builder.Property(b => b.LinkedinUrl).HasMaxLength(2048);

        builder
            .HasIndex(b => b.Name)
            .IsUnique();

        builder
            .HasIndex(b => b.Slug)
            .IsUnique();

        builder
            .HasMany(b => b.Memberships)
            .WithOne(m => m.Business)
            .HasForeignKey(m => m.BusinessId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(b => b.JobPosts)
            .WithOne(jp => jp.Business)
            .HasForeignKey(jp => jp.BusinessId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}