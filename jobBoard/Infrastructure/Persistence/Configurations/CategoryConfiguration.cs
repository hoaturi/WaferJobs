using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Label).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Slug);

        var categoryJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/categories.json"
        );

        var categories = categoryJson.DeserializeCaseInsensitive<List<Category>>();

        builder.HasData(categories);
    }
}
