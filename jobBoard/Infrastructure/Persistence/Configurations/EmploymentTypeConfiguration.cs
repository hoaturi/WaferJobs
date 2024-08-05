using JobBoard.Domain.JobPost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        var employmentTypes = JsonSerializer.Deserialize<List<EmploymentTypeEntity>>(employmentTypeJson);

        if (employmentTypes is null || employmentTypes.Count == 0)
            throw new Exception("Employment types data is missing or empty");

        builder.HasData(employmentTypes);
    }
}