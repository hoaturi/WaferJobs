﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaferJobs.Domain.Common;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<CityEntity>
{
    public void Configure(EntityTypeBuilder<CityEntity> builder)
    {
        builder.Property(c => c.Label).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(50);

        builder.HasIndex(c => c.Label).IsUnique();
        builder.HasIndex(c => c.Slug).IsUnique();
    }
}