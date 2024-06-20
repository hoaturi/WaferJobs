using JobBoard.Common.Extensions;
using JobBoard.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobBoard.Infrastructure.Persistence.Configurations;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRoleEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationRoleEntity> builder)
    {
        var roleJson = File.ReadAllText(
            "Infrastructure/Persistence/Configurations/SeedData/roles.json"
        );

        var roles = roleJson.DeserializeWithCaseInsensitivity<List<ApplicationRoleEntity>>();


        builder.HasData(roles);
    }
}