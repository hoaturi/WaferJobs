using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        var roleJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/roles.json"
        );

        var roles = roleJson.DeserializeCaseInsensitive<List<ApplicationRole>>();

        foreach (var role in roles)
        {
            role.Id = Guid.NewGuid();
        }

        builder.HasData(roles);
    }
}
