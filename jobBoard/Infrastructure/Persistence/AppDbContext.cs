using JobBoard.Common;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Domain.JobPost;
using JobBoard.Domain.JobPostEntities;
using JobBoard.Domain.JobSeeker;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUserEntity, ApplicationRoleEntity, Guid>(options)
{
    public DbSet<JobSeekerEntity> JobSeekers { get; init; }
    public DbSet<BusinessEntity> Businesses { get; init; }
    public DbSet<BusinessSizeEntity> BusinessSizes { get; init; }
    public DbSet<JobPostEntity> JobPosts { get; init; }
    public DbSet<CountryEntity> Countries { get; init; }
    public DbSet<CityEntity> Cities { get; init; }
    public DbSet<CategoryEntity> Categories { get; init; }
    public DbSet<EmploymentTypeEntity> EmploymentTypes { get; init; }
    public DbSet<JobPostPaymentEntity> JobPostPayments { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(e => e is { Entity: BaseEntity, State: EntityState.Added or EntityState.Modified });

        foreach (var entry in modifiedEntries)
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added) entity.CreatedAt = DateTime.UtcNow;

            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}