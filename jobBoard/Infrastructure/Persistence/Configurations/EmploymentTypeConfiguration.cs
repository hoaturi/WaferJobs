using JobBoard.Common.Extensions;
using JobBoard.Domain.JobPostEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class EmploymentTypeConfiguration : IEntityTypeConfiguration<EmploymentTypeEntity>
{
    public void Configure(EntityTypeBuilder<EmploymentTypeEntity> builder)
    {
        builder.Property(et => et.Label).IsRequired().HasMaxLength(20);
        builder.Property(et => et.Slug).IsRequired().HasMaxLength(20);
        builder.HasIndex(et => et.Slug);

        var employmentTypeJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/employmentTypes.json"
        );

        var employmentTypes = employmentTypeJson.DeserializeWithCaseInsensitivity<List<EmploymentTypeEntity>>();

        builder.HasData(employmentTypes);
    }
}