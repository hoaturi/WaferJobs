﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaferJobs.Domain.Common;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<TagEntity>

{
    public void Configure(EntityTypeBuilder<TagEntity> builder)
    {
        builder.Property(t => t.Label).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Slug).IsRequired().HasMaxLength(50);

        builder.HasIndex(t => t.Label).IsUnique();
        builder.HasIndex(t => t.Slug).IsUnique();
    }
}