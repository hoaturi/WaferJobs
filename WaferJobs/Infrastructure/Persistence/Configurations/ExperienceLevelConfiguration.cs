using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using WaferJobs.Domain.Common;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class ExperienceLevelConfiguration : IEntityTypeConfiguration<ExperienceLevelEntity>
{
    public void Configure(EntityTypeBuilder<ExperienceLevelEntity> builder)
    {
        builder.Property(e => e.Label).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(50);

        builder.HasIndex(e => e.Slug);

        var experienceLevelsJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/experienceLevels.json"
        );

        var experienceLevels = JsonConvert.DeserializeObject<List<ExperienceLevelEntity>>(experienceLevelsJson);

        if (experienceLevels is null || experienceLevels.Count == 0)
            throw new Exception("Experience levels data is missing or empty");

        builder.HasData(experienceLevels);
    }
}