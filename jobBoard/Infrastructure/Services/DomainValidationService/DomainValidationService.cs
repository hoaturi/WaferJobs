using System.Reflection;

namespace JobBoard.Infrastructure.Services.DomainValidationService;

public class DomainValidationService(ILogger<DomainValidationService> logger) : IDomainValidationService
{
    private const string FileName = "publicEmailDomains.txt";
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private HashSet<string>? _publicDomains;

    public async Task<bool> IsPublicEmailDomainAsync(string domain)
    {
        await EnsureDomainsLoadedAsync();
        return _publicDomains!.Contains(domain.ToLowerInvariant());
    }

    private async Task EnsureDomainsLoadedAsync()
    {
        if (_publicDomains is not null) return;

        await _semaphore.WaitAsync();
        try
        {
            if (_publicDomains is not null) return;
            await LoadPublicEmailDomainsAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task LoadPublicEmailDomainsAsync()
    {
        var domains = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        var domainTxtPath = resourceNames.FirstOrDefault(r => r.EndsWith(FileName))
                            ?? throw new FileNotFoundException(FileName);

        var domainTxt = Assembly.GetExecutingAssembly().GetManifestResourceStream(domainTxtPath);
        using var reader = new StreamReader(domainTxt!);
        while (await reader.ReadLineAsync() is { } line)
            domains.Add(line);

        _publicDomains = domains;
        logger.LogInformation("Public email domains loaded successfully. {DomainCount} domains loaded.",
            domains.Count);
    }
}