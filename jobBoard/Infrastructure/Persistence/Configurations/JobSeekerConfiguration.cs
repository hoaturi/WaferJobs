﻿using JobBoard.Domain.Business;
using JobBoard.Domain.JobSeeker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class JobSeekerConfiguration : IEntityTypeConfiguration<JobSeekerEntity> 
{
    public void Configure(EntityTypeBuilder<JobSeekerEntity> builder)
    {
        builder.Property(j => j.Name).IsRequired().HasMaxLength(50);
        builder.Property(j => j.UserId).IsRequired();

        
    }
}