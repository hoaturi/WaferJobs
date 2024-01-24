using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard;

public class BusinessSizeConfiguration : IEntityTypeConfiguration<BusinessSize>
{
    public void Configure(EntityTypeBuilder<BusinessSize> builder)
    {
        builder.Property(b => b.Name).IsRequired().HasMaxLength(20);

        var businessSizeJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/businessSizes.json"
        );

        var businessSizes = businessSizeJson.DeserializeCaseInsensitive<List<BusinessSize>>();

        builder.HasData(businessSizes);
    }
}
