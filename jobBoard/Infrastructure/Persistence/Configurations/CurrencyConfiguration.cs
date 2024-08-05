using JobBoard.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<CurrencyEntity>
{
    public void Configure(EntityTypeBuilder<CurrencyEntity> builder)
    {
        builder.Property(c => c.Label).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(3);
        builder.HasIndex(c => c.Code).IsUnique();

        var currenciesJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/currencies.json"
        );

        var currencies = JsonConvert.DeserializeObject<List<CurrencyEntity>>(currenciesJson);

        if (currencies is null) throw new Exception("Currencies data is missing or empty");

        builder.HasData(currencies);
    }
}