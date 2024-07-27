using JobBoard.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<CountryEntity>
{
    public void Configure(EntityTypeBuilder<CountryEntity> builder)
    {
        builder.Property(c => c.Label).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(2);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(50);

        builder.HasIndex(c => c.Slug);

        var countryJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/countries.json"
        );

        var countries = JsonSerializer.Deserialize<List<CountryEntity>>(countryJson);

        if (countries is null || countries.Count == 0)
            throw new Exception("Countries data is missing or empty");

        builder.HasData(countries);
    }
}