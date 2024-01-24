using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard;

public class EmploymentTypeConfiguration : IEntityTypeConfiguration<EmploymentType>
{
    public void Configure(EntityTypeBuilder<EmploymentType> builder)
    {
        builder.Property(et => et.Name).IsRequired().HasMaxLength(20);

        var employmentTypeJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/employmentTypes.json"
        );

        var employmentTypes = employmentTypeJson.DeserializeCaseInsensitive<List<EmploymentType>>();

        builder.HasData(employmentTypes);
    }
}
