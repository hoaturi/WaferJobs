using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaferJobs.Domain.Auth;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WaferJobs.Infrastructure.Persistence.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRoleEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleEntity> builder)
    {
        var roleJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/roles.json"
        );

        var roles = JsonSerializer.Deserialize<List<ApplicationRoleEntity>>(roleJson);

        if (roles is null || roles.Count == 0)
            throw new Exception("Roles data is missing or empty");

        builder.HasData(roles);
    }
}