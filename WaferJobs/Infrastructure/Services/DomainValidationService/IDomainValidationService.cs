namespace WaferJobs.Infrastructure.Services.DomainValidationService;

public interface IDomainValidationService
{
    Task<bool> IsPublicEmailDomainAsync(string domain);
}