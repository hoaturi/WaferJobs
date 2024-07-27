using JobBoard.Domain.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class BusinessSizeConfiguration : IEntityTypeConfiguration<BusinessSizeEntity>
{
    public void Configure(EntityTypeBuilder<BusinessSizeEntity> builder)
    {
        builder.Property(b => b.Label).IsRequired().HasMaxLength(20);

        var businessSizeJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/businessSizes.json"
        );

        var businessSizes = JsonSerializer.Deserialize<List<BusinessSizeEntity>>(businessSizeJson);

        if (businessSizes is null || businessSizes.Count == 0)
            throw new Exception("Business sizes data is missing or empty");

        builder.HasData(businessSizes);
    }
}