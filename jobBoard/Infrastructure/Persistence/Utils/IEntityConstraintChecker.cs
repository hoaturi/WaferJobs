namespace JobBoard.Infrastructure.Persistence.Utils;

public interface IEntityConstraintChecker
{
    bool IsUniqueConstraintViolation<TEntity>(
        string propertyName,
        string errorCode,
        string? constraintName)
        where TEntity : class;
}