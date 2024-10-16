using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using WaferJobs.Domain.JobPost;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<CategoryEntity>
{
    public void Configure(EntityTypeBuilder<CategoryEntity> builder)
    {
        builder.Property(c => c.Label).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Slug);

        var categoryJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/categories.json"
        );

        var categories = JsonConvert.DeserializeObject<List<CategoryEntity>>(categoryJson);

        if (categories is null) throw new Exception("Categories data is missing or empty");

        builder.HasData(categories);
    }
}