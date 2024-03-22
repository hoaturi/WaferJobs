using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.Property(c => c.Label).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(2);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Slug);

        var countryJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/countries.json"
        );

        var countries = countryJson.DeserializeCaseInsensitive<List<Country>>();

        foreach (var country in countries)
        {
            country.Id = countries.IndexOf(country) + 1;
        }

        builder.HasData(countries);
    }
}
