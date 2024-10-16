using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WaferJobs.Infrastructure.Persistence.Utils;

public class EntityConstraintChecker(ILogger<EntityConstraintChecker> logger, AppDbContext dbContext)
    : IEntityConstraintChecker
{
    public bool IsUniqueConstraintViolation<TEntity>(string propertyName, string errorCode, string? constraintName)
        where TEntity : class
    {
        if (errorCode != PostgresErrorCodes.UniqueViolation) return false;

        var index = dbContext.Model.FindEntityType(typeof(TEntity))
            ?.GetIndexes()
            .FirstOrDefault(i => i.IsUnique && i.Properties.Any(p => p.Name == propertyName));

        var databaseIndexName = index?.GetDatabaseName();
        logger.LogDebug("{EntityName} unique index name for {PropertyName}: {IndexName}",
            typeof(TEntity).Name, propertyName, databaseIndexName);

        return databaseIndexName == constraintName;
    }
}