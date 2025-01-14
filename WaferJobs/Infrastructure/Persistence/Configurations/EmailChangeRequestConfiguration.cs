﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaferJobs.Domain.Auth;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class EmailChangeRequestConfiguration : IEntityTypeConfiguration<EmailChangeRequestEntity>
{
    public void Configure(EntityTypeBuilder<EmailChangeRequestEntity> builder)
    {
        builder.HasIndex(ecr => ecr.Pin)
            .IsUnique();

        builder.HasIndex(ecr => ecr.ExpiresAt);
        builder.HasIndex(ecr => ecr.IsVerified);

        builder.HasOne(ecr => ecr.User)
            .WithMany()
            .HasForeignKey(ecr => ecr.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}