using JobBoard.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class BusinessSizeConfiguration : IEntityTypeConfiguration<BusinessSizeEntity>
{
    public void Configure(EntityTypeBuilder<BusinessSizeEntity> builder)
    {
        builder.Property(b => b.Name).IsRequired().HasMaxLength(20);

        var businessSizeJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/businessSizes.json"
        );

        var businessSizes = businessSizeJson.DeserializeWithCaseInsensitivity<List<BusinessSizeEntity>>();

        builder.HasData(businessSizes);
    }
}