using JobBoard.Common;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Domain.JobAlert;
using JobBoard.Domain.JobPost;
using JobBoard.Domain.JobSeeker;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUserEntity, ApplicationRoleEntity, Guid>(options)
{
    public DbSet<JobSeekerEntity> JobSeekers { get; set; }
    public DbSet<BusinessEntity> Businesses { get; set; }
    public DbSet<BusinessSizeEntity> BusinessSizes { get; set; }
    public DbSet<JobPostEntity> JobPosts { get; set; }
    public DbSet<CountryEntity> Countries { get; set; }
    public DbSet<CityEntity> Cities { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<EmploymentTypeEntity> EmploymentTypes { get; set; }
    public DbSet<JobPostPaymentEntity> JobPostPayments { get; set; }
    public DbSet<JobAlertEntity> JobAlerts { get; set; }

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