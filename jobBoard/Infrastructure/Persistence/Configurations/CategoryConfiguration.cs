using JobBoard.Common.Extensions;
using JobBoard.Domain.JobPost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

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

        var categories = categoryJson.DeserializeWithCaseInsensitivity<List<CategoryEntity>>();

        builder.HasData(categories);
    }
}